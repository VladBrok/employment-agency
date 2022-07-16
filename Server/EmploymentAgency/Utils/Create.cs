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
        return $@"INSERT INTO {table} ({string.Join(", ", keys)}) 
        VALUES ({string.Join(", ", values.Select(v => DefaultIfEmpty(v)))})";
    }

    public string Addresses(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO {table} (street_id, building_number) 
        VALUES ('{entity["street_id"]}', '{entity["building_number"]}')";
    }

    public string Employers(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO addresses(street_id, building_number) 
            VALUES('{entity["street_id"]}', '{entity["building_number"]}');
        INSERT INTO {table} (employer, email, phone, property_id, address_id)
        VALUES('{entity["employer"]}', {DefaultIfEmpty(entity["email"])}, 
        {DefaultIfEmpty(entity["phone"])}, '{entity["property_id"]}', (SELECT max(id) FROM addresses))";
    }

    public string Seekers(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO addresses(street_id, building_number) VALUES('{entity["street_id"]}', '{entity["building_number"]}');
        INSERT INTO {table} (first_name, last_name, patronymic, phone, birthday, registration_city, 
            recommended, pol, education, status_id, speciality_id, address_id)
        VALUES('{entity["first_name"]}',
        '{entity["last_name"]}',
        {DefaultIfEmpty(entity["patronymic"])},
        {DefaultIfEmpty(entity["phone"])},
        {DefaultIfEmpty(entity["birthday"])},
        {DefaultIfEmpty(entity["registration_city"])},
        {DefaultIfEmpty(entity["recommended"])},
        {DefaultIfEmpty(entity["pol"])},
        {DefaultIfEmpty(entity["education"])},
        {entity["status_id"]},
        {entity["speciality_id"]},
        (SELECT max(id) FROM addresses))";
    }
}
