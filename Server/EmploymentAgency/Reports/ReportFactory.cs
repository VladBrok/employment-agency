namespace EmploymentAgency.Reports;

public static class ReportFactory
{
    public static IReport MakeReport(string type)
    {
        return type.ToLower() switch
        {
            "html" => new HtmlReport(),
            "excel" => new ExcelReport(),
            _ => throw new ArgumentException(type)
        };
    }
}
