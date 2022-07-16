namespace EmploymentAgency.Utils;

public class Update : CrudAction
{
    public Update(
        Dictionary<string, IEnumerable<IEnumerable<string>>> fileSignatures,
        int maxFileSize
    ) : base(fileSignatures, maxFileSize) { }

    public string Addresses(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"UPDATE streets SET district_id = {entity["district_id"]} WHERE id = {entity["street_id"]};
        UPDATE {table} SET street_id = {entity["street_id"]}, building_number = {entity["building_number"]}";
    }

    public string Employers(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO addresses(street_id, building_number) 
            VALUES('{entity["street_id"]}', '{entity["building_number"]}');
        UPDATE {table} SET employer = '{entity["employer"]}',
            email = {DefaultIfEmpty(entity["email"])},
            phone = {DefaultIfEmpty(entity["phone"])},
            property_id = '{entity["property_id"]}',
            address_id = (SELECT max(id) FROM addresses)";
    }

    public string Seekers(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO addresses(street_id, building_number) 
            VALUES('{entity["street_id"]}', '{entity["building_number"]}');
        UPDATE {table} SET first_name = '{entity["first_name"]}',
            last_name = '{entity["last_name"]}',
            patronymic = {DefaultIfEmpty(entity["patronymic"])},
            phone = {DefaultIfEmpty(entity["phone"])},
            birthday = {DefaultIfEmpty(entity["birthday"])},
            registration_city = {DefaultIfEmpty(entity["registration_city"])},
            recommended = {DefaultIfEmpty(entity["recommended"])},
            pol = {DefaultIfEmpty(entity["pol"])},
            education = {DefaultIfEmpty(entity["education"])},
            status_id = {entity["status_id"]},
            speciality_id = {entity["speciality_id"]},
            address_id = (SELECT max(id) FROM addresses)";
    }

    protected override string PerformOnTable(
        string table,
        IEnumerable<string> keys,
        IEnumerable<string> values
    )
    {
        return $@"UPDATE {table} 
        SET {string.Join(", ", keys.Zip(values).Select(e => $"{e.First} = {DefaultIfEmpty(e.Second)}"))}";
    }
}
