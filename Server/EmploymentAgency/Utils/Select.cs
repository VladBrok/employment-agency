namespace EmploymentAgency.Utils;

public static class Select
{
    public static string FromDistricts()
    {
        return $@"SELECT d.id, d.district, c.city
        FROM districts d
        JOIN cities c on c.id = d.city_id";
    }

    public static string FromEmployers()
    {
        return $@"SELECT e.id, e.employer, p.property, 
              e.phone, e.email, c.city, d.district
       FROM employers e
       JOIN properties p ON p.id = e.property_id
       JOIN districts d ON d.id = e.district_id
       JOIN cities c ON c.id = d.city_id";
    }

    public static string FromSeekers()
    {
        return $@"SELECT s.id, s.first_name, s.last_name, s.patronymic,
               stat.status, e.education,
               s.phone, s.birthday, s.pol, c.city as registration_city,
               cc.city, d.district
        FROM seekers s
        JOIN statuses stat ON stat.id = s.status_id
        JOIN districts d ON d.id = s.district_id
        JOIN cities c ON c.id = s.registration_city_id
        JOIN cities cc ON cc.id = d.city_id
        JOIN educations e ON e.id = s.education_id";
    }

    public static string FromVacancies()
    {
        return $@"SELECT v.id, e.employer employer_company, v.employer_id, p.position, v.employer_day,
               v.salary_new, v.chart_new, v.vacancy_end
        FROM vacancies v
        JOIN positions p ON p.id = v.position_id
        JOIN employers e ON e.id = v.employer_id";
    }

    public static string FromApplications()
    {
        return $@"SELECT a.id, s.first_name seeker_name, a.seeker_id, p.position, et.type,
                  a.seeker_day, a.information, a.photo, a.salary, a.experience, a.recommended
        FROM applications a
        JOIN positions p ON p.id = a.position_id
        JOIN employment_types et ON et.id = a.employment_type_id
        JOIN seekers s ON s.id = a.seeker_id";
    }

    public static string From(string target)
    {
        return $"SELECT * FROM {target}";
    }
}
