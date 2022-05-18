namespace EmploymentAgency.Reports;

public interface IReport
{
    Task BuildAsync(IEnumerable<Entity> entities, string outputFile, string title);
}
