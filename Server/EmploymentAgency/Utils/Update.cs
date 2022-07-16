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
        return $@"UPDATE streets SET district_id = {Escape(entity["district_id"])} WHERE id = {Escape(entity["street_id"])};
        UPDATE {table} SET street_id = {Escape(entity["street_id"])}, building_number = {Escape(entity["building_number"])}";
    }

    public string Employers(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO addresses(street_id, building_number) 
            VALUES('{Escape(entity["street_id"])}', '{Escape(entity["building_number"])}');
        UPDATE {table} SET employer = '{Escape(entity["employer"])}',
            email = {DefaultIfEmpty(Escape(entity["email"]))},
            phone = {DefaultIfEmpty(Escape(entity["phone"]))},
            property_id = '{Escape(entity["property_id"])}',
            address_id = (SELECT max(id) FROM addresses)";
    }

    public string Seekers(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO addresses(street_id, building_number) 
            VALUES('{Escape(entity["street_id"])}', '{Escape(entity["building_number"])}');
        UPDATE {table} SET first_name = '{Escape(entity["first_name"])}',
            last_name = '{Escape(entity["last_name"])}',
            patronymic = {DefaultIfEmpty(Escape(entity["patronymic"]))},
            phone = {DefaultIfEmpty(Escape(entity["phone"]))},
            birthday = {DefaultIfEmpty(Escape(entity["birthday"]))},
            registration_city = {DefaultIfEmpty(Escape(entity["registration_city"]))},
            recommended = {DefaultIfEmpty(Escape(entity["recommended"]))},
            pol = {DefaultIfEmpty(Escape(entity["pol"]))},
            education = {DefaultIfEmpty(Escape(entity["education"]))},
            status_id = {Escape(entity["status_id"])},
            speciality_id = {Escape(entity["speciality_id"])},
            address_id = (SELECT max(id) FROM addresses)";
    }

    protected override string PerformOnTable(
        string table,
        IEnumerable<string> keys,
        IEnumerable<string> values
    )
    {
        return $@"UPDATE {table} 
        SET {string.Join(", ", keys.Zip(values).Select(e => $"{Escape(e.First)} = {DefaultIfEmpty(Escape(e.Second))}"))}";
    }
}
