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
       JOIN districts d ON d.id = a.district_id
       JOIN streets s ON d.id = s.district_id";
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
        JOIN districts d ON d.id = a.district_id
        JOIN streets st ON d.id = st.district_id";
    }
    public static string FromAddresses()
    {
        return $@"SELECT a.id, d.district, s.street, a.building_number, s.postal_code
        FROM addresses a
        JOIN districts d ON d.id = a.district_id
        JOIN streets s ON d.id = s.district_id";
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
