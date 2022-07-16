namespace EmploymentAgency.Utils;

public class Create : CrudAction
{
    public Create(
        Dictionary<string, IEnumerable<IEnumerable<string>>> fileSignatures,
        int maxFileSize
    ) : base(fileSignatures, maxFileSize) { }

    protected override string PerformOnTable(
        string table,
        IEnumerable<string> keys,
        IEnumerable<string> values
    )
    {
        return $@"INSERT INTO {table} ({Escape(string.Join(", ", keys))}) 
        VALUES ({string.Join(", ", values.Select(v => DefaultIfEmpty(Escape(v))))})";
    }

    public string Addresses(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO {table} (street_id, building_number) 
        VALUES ('{Escape(entity["street_id"])}', '{Escape(entity["building_number"])}')";
    }

    public string Employers(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO addresses(street_id, building_number) 
            VALUES('{Escape(entity["street_id"])}', '{Escape(entity["building_number"])}');
        INSERT INTO {table} (employer, email, phone, property_id, address_id)
        VALUES('{Escape(entity["employer"])}', {DefaultIfEmpty(Escape(entity["email"]))}, 
        {DefaultIfEmpty(Escape(entity["phone"]))}, '{Escape(entity["property_id"])}', (SELECT max(id) FROM addresses))";
    }

    public string Seekers(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO addresses(street_id, building_number) VALUES('{Escape(entity["street_id"])}', '{Escape(entity["building_number"])}');
        INSERT INTO {table} (first_name, last_name, patronymic, phone, birthday, registration_city, 
            recommended, pol, education, status_id, speciality_id, address_id)
        VALUES('{Escape(entity["first_name"])}',
        '{Escape(entity["last_name"])}',
        {DefaultIfEmpty(Escape(entity["patronymic"]))},
        {DefaultIfEmpty(Escape(entity["phone"]))},
        {DefaultIfEmpty(Escape(entity["birthday"]))},
        {DefaultIfEmpty(Escape(entity["registration_city"]))},
        {DefaultIfEmpty(Escape(entity["recommended"]))},
        {DefaultIfEmpty(Escape(entity["pol"]))},
        {DefaultIfEmpty(Escape(entity["education"]))},
        {Escape(entity["status_id"])},
        {Escape(entity["speciality_id"])},
        (SELECT max(id) FROM addresses))";
    }
}
