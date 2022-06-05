namespace EmploymentAgency.Utils;

public static class Default
{
    public static string IfEmpty(string value)
    {
        return value == "" ? "DEFAULT" : $"'{value}'";
    }
}
