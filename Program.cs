//1
public interface IReport
{
    string Generate(); // Генерация отчета
}
public class SalesReport : IReport
{
    public string Generate()
    {
        // Фиктивные данные для отчета по продажам
        return "Отчет о продажах: \n1. Продукт А - 100 единиц \n2. Продукт В - 200 единиц";
    }
}

public class UserReport : IReport
{
    public string Generate()
    {
        // Фиктивные данные для отчета по пользователям
        return "Отчет пользователя: \n1. Пользователь A \n2. Пользователь B";
    }
}
public abstract class ReportDecorator : IReport
{
    protected IReport _report;

    public ReportDecorator(IReport report)
    {
        _report = report;
    }

    public virtual string Generate()
    {
        return _report.Generate();
    }
}
public class DateFilterDecorator : ReportDecorator
{
    private DateTime _startDate;
    private DateTime _endDate;

    public DateFilterDecorator(IReport report, DateTime startDate, DateTime endDate)
        : base(report)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override string Generate()
    {
        return base.Generate() + $"\nOтфильтровано по дате: {_startDate.ToShortDateString()} к {_endDate.ToShortDateString()}";
    }
}
public class SortingDecorator : ReportDecorator
{
    private string _sortBy;

    public SortingDecorator(IReport report, string sortBy) : base(report)
    {
        _sortBy = sortBy;
    }

    public override string Generate()
    {
        return base.Generate() + $"\nОтсортированный по: {_sortBy}";
    }
}
public class CsvExportDecorator : ReportDecorator
{
    public CsvExportDecorator(IReport report) : base(report) { }

    public override string Generate()
    {
        string data = base.Generate();
        return $"Экспорт отчета в формат CSV:\n{data.Replace("\n", ",")}";
    }
}
public class PdfExportDecorator : ReportDecorator
{
    public PdfExportDecorator(IReport report) : base(report) { }

    public override string Generate()
    {
        return $"Экспорт отчета в формат PDF:\n{base.Generate()}";
    }
}
class Program
{
    static void Main(string[] args)
    {
        // Создаем отчет по продажам
        IReport report = new SalesReport();
        Console.WriteLine(report.Generate());

        // Добавляем фильтрацию по датам
        report = new DateFilterDecorator(report, new DateTime(2023, 1, 1), new DateTime(2023, 12, 31));
        Console.WriteLine(report.Generate());

        // Добавляем сортировку по дате
        report = new SortingDecorator(report, "Дата");
        Console.WriteLine(report.Generate());

        // Экспорт в CSV
        report = new CsvExportDecorator(report);
        Console.WriteLine(report.Generate());

        // Экспорт в PDF
        report = new PdfExportDecorator(report);
        Console.WriteLine(report.Generate());
    }
}

//2
public interface IInternalDeliveryService
{
    void DeliverOrder(string orderId); // Доставка заказа
    string GetDeliveryStatus(string orderId); // Статус доставки
}
public class InternalDeliveryService : IInternalDeliveryService
{
    public void DeliverOrder(string orderId)
    {
        Console.WriteLine($"Внутренняя доставка: Доставка заказа {orderId}.");
    }

    public string GetDeliveryStatus(string orderId)
    {
        return $"Внутренняя доставка: Заказ {orderId} доставляется.";
    }
}
public class ExternalLogisticsServiceA
{
    public void ShipItem(int itemId)
    {
        Console.WriteLine($"Внешняя логистическая услуга A: Доставка товара {itemId}.");
    }

    public string TrackShipment(int shipmentId)
    {
        return $"Внешняя логистическая служба A: Отгрузка {shipmentId} в пути.";
    }
}
public class ExternalLogisticsServiceB
{
    public void SendPackage(string packageInfo)
    {
        Console.WriteLine($"Внешняя логистическая служба B: Отправка посылки с информацией {packageInfo}.");
    }

    public string CheckPackageStatus(string trackingCode)
    {
        return $"Внешняя логистическая служба B: Посылка {trackingCode} доставлена.";
    }
}
public class LogisticsAdapterA : IInternalDeliveryService
{
    private ExternalLogisticsServiceA _externalService;

    public LogisticsAdapterA(ExternalLogisticsServiceA externalService)
    {
        _externalService = externalService;
    }

    public void DeliverOrder(string orderId)
    {
        int itemId = int.Parse(orderId); // Преобразуем orderId в itemId
        _externalService.ShipItem(itemId);
    }

    public string GetDeliveryStatus(string orderId)
    {
        int shipmentId = int.Parse(orderId); // Преобразуем orderId в shipmentId
        return _externalService.TrackShipment(shipmentId);
    }
}
public class LogisticsAdapterB : IInternalDeliveryService
{
    private ExternalLogisticsServiceB _externalService;

    public LogisticsAdapterB(ExternalLogisticsServiceB externalService)
    {
        _externalService = externalService;
    }

    public void DeliverOrder(string orderId)
    {
        _externalService.SendPackage(orderId);
    }

    public string GetDeliveryStatus(string orderId)
    {
        return _externalService.CheckPackageStatus(orderId);
    }
}
public class DeliveryServiceFactory
{
    public static IInternalDeliveryService GetDeliveryService(string serviceType)
    {
        if (serviceType == "Internal")
        {
            return new InternalDeliveryService();
        }
        else if (serviceType == "ExternalA")
        {
            return new LogisticsAdapterA(new ExternalLogisticsServiceA());
        }
        else if (serviceType == "ExternalB")
        {
            return new LogisticsAdapterB(new ExternalLogisticsServiceB());
        }
        throw new ArgumentException("Unknown service type.");
    }
}
class Program
{
    static void Main(string[] args)
    {
        IInternalDeliveryService deliveryService;

        // Внутренняя доставка
        deliveryService = DeliveryServiceFactory.GetDeliveryService("Internal");
        deliveryService.DeliverOrder("1001");
        Console.WriteLine(deliveryService.GetDeliveryStatus("1001"));

        // Внешняя служба A
        deliveryService = DeliveryServiceFactory.GetDeliveryService("ExternalA");
        deliveryService.DeliverOrder("2001");
        Console.WriteLine(deliveryService.GetDeliveryStatus("2001"));

        // Внешняя служба B
        deliveryService = DeliveryServiceFactory.GetDeliveryService("ExternalB");
        deliveryService.DeliverOrder("Package001");
        Console.WriteLine(deliveryService.GetDeliveryStatus("Package001"));
    }
}
