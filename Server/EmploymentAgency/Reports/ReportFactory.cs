namespace EmploymentAgency.Reports;

public class ReportFactory
{
    public IReport MakeReport(string type)
    {
        return type.ToLower() switch
        {
            "html" => new HtmlReport(),
            "excel" => new ExcelReport(),
            _ => throw new ArgumentException(type)
        };
    }
}
