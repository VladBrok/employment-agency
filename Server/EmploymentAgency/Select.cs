namespace EmploymentAgency;

public static class Select
{
    public static string FromEmployers()
    {
        return $@"SELECT e.id, e.employer, p.property, 
              e.phone, e.email, d.district, 
              s.street, a.building_number, s.postal_code
       FROM employers e
       JOIN properties p ON p.id = e.property_id
       JOIN addresses a ON a.id = e.address_id
       JOIN streets s ON s.id = a.street_id
       JOIN districts d ON d.id = s.district_id";
    }

    public static string FromSeekers()
    {
        return $@"SELECT s.id, s.first_name, s.last_name, s.patronymic,
               stat.status, p.position, s.education,
               s.phone, s.birthday, s.registration_city,
               s.recommended, s.pol, d.district,
               st.street, a.building_number, st.postal_code
        FROM seekers s
        JOIN statuses stat ON stat.id = s.status_id
        JOIN positions p ON p.id = s.speciality_id
        JOIN addresses a ON a.id = s.address_id
        JOIN streets st ON st.id = a.street_id
        JOIN districts d ON d.id = st.district_id";
    }

    public static string FromAddresses()
    {
        return $@"SELECT a.id, d.district, s.street, a.building_number, s.postal_code
        FROM addresses a
        JOIN streets s ON s.id = a.street_id
        JOIN districts d ON d.id = s.district_id";
    }

    public static string FromVacancies()
    {
        return $@"SELECT v.id, v.employer_id, p.position, v.employer_day,
               v.salary_new, v.chart_new, v.vacancy_end
        FROM vacancies v
        JOIN positions p ON p.id = v.position_id";
    }

    public static string FromApplications()
    {
        return $@"SELECT a.id, a.seeker_id, p.position, et.type,
                  a.seeker_day, a.information, a.photo, a.experience
        FROM applications a
        JOIN positions p ON p.id = a.position_id
        JOIN employment_types et ON et.id = a.employment_type_id";
    }

    public static string FromStreets()
    {
        return $@"SELECT s.id, d.district, s.street, s.postal_code
        FROM streets s
        JOIN districts d ON d.id = s.district_id";
    }

    public static string From(string target)
    {
        return $"SELECT * FROM {target}";
    }
}

public static class Update
{
    public static string Table(string table, HttpRequest request)
    {
        return $@"UPDATE {table} 
        SET {string.Join(", ", request.Form.Select(e => $"{e.Key} = {Default.IfEmpty(e.Value)}"))}";
    }

    public static string Addresses(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"UPDATE streets SET district_id = {entity["district_id"]} WHERE id = {entity["street_id"]};
        UPDATE {table} SET street_id = {entity["street_id"]}, building_number = {entity["building_number"]}";
    }

    public static string Applications()
    {
        return "";
    }

    public static string Employers(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO addresses(street_id, building_number) VALUES('{entity["street_id"]}', '{entity["building_number"]}');
        UPDATE {table} SET employer = '{entity["employer"]}',
            email = {Default.IfEmpty(entity["email"])},
            phone = {Default.IfEmpty(entity["phone"])},
            property_id = '{entity["property_id"]}',
            address_id = (SELECT max(id) FROM addresses)";
    }

    public static string Seekers(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO addresses(street_id, building_number) VALUES('{entity["street_id"]}', '{entity["building_number"]}');
        UPDATE {table} SET first_name = '{entity["first_name"]}',
            last_name = '{entity["last_name"]}',
            patronymic = {Default.IfEmpty(entity["patronymic"])},
            phone = {Default.IfEmpty(entity["phone"])},
            birthday = {Default.IfEmpty(entity["birthday"])},
            registration_city = {Default.IfEmpty(entity["registration_city"])},
            recommended = {Default.IfEmpty(entity["recommended"])},
            pol = {Default.IfEmpty(entity["pol"])},
            education = {Default.IfEmpty(entity["education"])},
            status_id = {entity["status_id"]},
            speciality_id = {entity["speciality_id"]},
            address_id = (SELECT max(id) FROM addresses)";
    }

    public static string Vacancies()
    {
        return "";
    }
}

public static class Create
{
    public static string Table(string table, HttpRequest request)
    {
        var entity = request.Form;
        return $@"INSERT INTO {table} ({string.Join(", ", entity.Select(e => e.Key))}) 
        VALUES ('{string.Join("', '", entity.Select(e => e.Value))}')";
    }
}

public static class Default
{
    public static string IfEmpty(string value)
    {
        return value == "" ? "DEFAULT" : $"'{value}'";
    }
}
