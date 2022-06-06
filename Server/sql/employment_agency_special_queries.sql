DROP VIEW IF EXISTS average_seeker_ages_by_positions;
DROP VIEW IF EXISTS employer_addresses;
DROP VIEW IF EXISTS employment_types_and_salaries;
DROP VIEW IF EXISTS vacancies_and_salaries;
DROP VIEW IF EXISTS employers_and_vacancies;
DROP VIEW IF EXISTS seekers_and_applications;
DROP VIEW IF EXISTS num_vacancies_from_each_employer;

DROP FUNCTION IF EXISTS get_applications_percent_after;
DROP FUNCTION IF EXISTS get_applications_percent_by_positions_after;
DROP FUNCTION IF EXISTS get_application_count_by_positions;
DROP FUNCTION IF EXISTS get_seekers_in_district;
DROP FUNCTION IF EXISTS get_employers_with_property;
DROP FUNCTION IF EXISTS get_vacancies_posted_on;
DROP FUNCTION IF EXISTS get_seekers_born_after;
DROP FUNCTION IF EXISTS get_applications_without_experience;
DROP FUNCTION IF EXISTS get_max_salaries_for_position;
DROP FUNCTION IF EXISTS get_seekers_whose_total_experience_exceeds;
DROP FUNCTION IF EXISTS get_positions_from_open_vacancies_whose_average_salary_exceeds;
DROP FUNCTION IF EXISTS get_num_applications_for_each_employment_type;
DROP FUNCTION IF EXISTS get_latest_vacancy_of_employers_whose_name_contains;



/* ===== Special queries ===== */


CREATE OR REPLACE FUNCTION get_applications_percent_after(a_year INT)
    RETURNS TABLE("% заявок" NUMERIC(2, 1)) AS
$$
BEGIN
    RETURN QUERY 
        SELECT 
            round((((sum(CASE 
                     WHEN EXTRACT(YEAR FROM seeker_day) > a_year THEN 1 
                     ELSE 0 
                 END) OVER()) * 100)::float / (count(*) OVER()))::numeric, 2)
        FROM applications
        LIMIT 1;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION get_applications_percent_by_positions_after(a_year INT)
    RETURNS TABLE("Должность" citext, "% заявок" NUMERIC(2, 1)) AS
$$
BEGIN
    RETURN QUERY
        SELECT p.position, round(((a_after.count * 100)::float / a_total.count)::numeric, 2)
        FROM positions p
        JOIN
        (
            SELECT count(id) count
            FROM applications
            WHERE EXTRACT(YEAR FROM seeker_day) > a_year
        ) a_total
            ON 1 = 1
        JOIN
        (
            SELECT position_id, count(position_id) count
            FROM applications
            WHERE EXTRACT(YEAR FROM seeker_day) > a_year
            GROUP BY position_id
        ) a_after
            ON p.id = a_after.position_id;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION get_application_count_by_positions(a_year INT, a_month INT)
    RETURNS TABLE("Должность" citext, "Количество заявок" BIGINT)
AS
$$
BEGIN
    RETURN QUERY
        SELECT p.position, a.count
        FROM
        (
            SELECT position_id, count(position_id) count
            FROM applications
            WHERE EXTRACT(YEAR FROM seeker_day) = a_year AND
                  EXTRACT(MONTH FROM seeker_day) = a_month
            GROUP BY position_id
        ) a
        JOIN positions p
            ON p.id = a.position_id;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE VIEW average_seeker_ages_by_positions AS
    SELECT p.position "Должность", t.age "Средний возраст"
    FROM positions p
    JOIN
    (
        SELECT 
            a.position_id,
            EXTRACT(YEAR FROM avg(age(s.birthday)))::NUMERIC age
        FROM seekers s
        JOIN applications a
            ON s.id = a.seeker_id
        GROUP BY a.position_id
    ) t
        ON p.id = t.position_id;
        

/*select '-----По всему агентству-----', null
union all
(select null, * from get_applications_percent_after(2016))
union all
(select '-----По должностям-----', null)
union all
(select * from get_applications_percent_by_positions_after(2016))*/


/* ===== Lab 6 ===== */


/* ===== 1 ===== */


CREATE OR REPLACE FUNCTION get_seekers_in_district(a_district VARCHAR(100))
    RETURNS TABLE(
        "Фамилия" VARCHAR(50), 
        "Имя" VARCHAR(50), 
        "Отчество" VARCHAR(50), 
        "Телефон" phone_number) AS
$$
BEGIN
    RETURN QUERY
        SELECT last_name, first_name, patronymic, phone
        FROM seekers s
        JOIN addresses a
            ON a.id = s.address_id
        JOIN streets st
            ON st.id = a.street_id
        JOIN districts d
            ON d.id = st.district_id
        WHERE lower(d.district) = lower(a_district)
        ORDER BY last_name, first_name, patronymic, phone;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION get_employers_with_property(a_property VARCHAR(30))
    RETURNS TABLE(
        "Компания" citext,
        "Телефон" phone_number,
        "Почта" email_address) AS
$$
BEGIN
    RETURN QUERY
        SELECT employer, phone, email
        FROM employers e
        JOIN properties p
            ON e.property_id = p.id
        WHERE lower(p.property) = lower(a_property)
        ORDER BY employer, phone, email;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION get_vacancies_posted_on(a_date DATE)
    RETURNS TABLE(
        "Должность" citext,
        "Зарплата" NUMERIC(8, 2)) AS
$$
BEGIN
    RETURN QUERY
        SELECT p.position, v.salary_new
        FROM vacancies v
        JOIN positions p
            ON v.position_id = p.id
        WHERE v.employer_day::DATE = a_date
        ORDER BY p.position, v.salary_new;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION get_seekers_born_after(a_date DATE)
    RETURNS TABLE(
        "Фамилия" VARCHAR(50), 
        "Имя" VARCHAR(50), 
        "Отчество" VARCHAR(50),
        "Дата рождения" DATE,
        "Образование" VARCHAR(50),
        "Специальность" citext) AS
$$
BEGIN
    RETURN QUERY
        SELECT 
            last_name, 
            first_name, 
            patronymic,
            birthday, 
            education, 
            p.position
        FROM seekers s
        JOIN positions p
            ON s.speciality_id = p.id
        WHERE s.birthday > a_date
        ORDER BY 1, 2, 3, 4, 5, 6;
END;
$$ LANGUAGE 'plpgsql' STRICT;


/* ===== 2 ===== */


CREATE OR REPLACE VIEW employer_addresses AS
    SELECT 
        e.employer "Компания", 
        d.district "Район", 
        s.street "Улица", 
        a.building_number "Номер дома"
    FROM employers e
    JOIN addresses a
        ON e.address_id = a.id
    JOIN streets s
        ON a.street_id = s.id
    JOIN districts d
        ON s.district_id = d.id
    ORDER BY 1, 2, 3, 4;


CREATE OR REPLACE VIEW employment_types_and_salaries AS
    SELECT DISTINCT
        e.type "Тип занятости",
        round(avg(a.salary)::numeric, 0) "Средняя зарплата",
        round(avg(a.experience)::numeric, 0) "Средний опыт"
    FROM applications a
    JOIN employment_types e
        ON a.employment_type_id = e.id
    GROUP BY e.type
    ORDER BY 1, 2;


CREATE OR REPLACE VIEW vacancies_and_salaries AS
    SELECT DISTINCT 
        p.position "Должность",
        v.chart_new "График",
        v.salary_new "Зарплата"
    FROM vacancies v
    JOIN positions p
        ON v.position_id = p.id
    ORDER BY 1, 2, 3;


/* ===== 3 ===== */


CREATE OR REPLACE VIEW employers_and_vacancies AS
    SELECT 
        e.employer "Компания", 
        v.employer_day "Дата размещения вакансии",
        p.position "Должность",
        salary_new "Зарплата"
    FROM employers e
    LEFT JOIN vacancies v
        ON e.id = v.employer_id
    LEFT JOIN positions p
        ON p.id = v.position_id
    ORDER BY e.employer, v.employer_day, p.position, v.salary_new;


/* ===== 4 ===== */


CREATE OR REPLACE VIEW seekers_and_applications AS
    SELECT
        last_name "Фамилия",
        first_name "Имя",
        patronymic "Отчество",
        registration_city "Город регистрации",
        seeker_day "Дата размещения заявки",
        salary "Ожидаемая зарплата"
    FROM applications a
    RIGHT JOIN seekers s
        ON s.id = a.seeker_id
    ORDER BY 1, 2, 3, 4, 5, 6;


/* ===== 5 ===== */


CREATE OR REPLACE FUNCTION get_applications_without_experience(a_position VARCHAR(50))
    RETURNS TABLE(
        "Тип занятости" citext,
        "Дата подачи заявления" TIMESTAMP,
        "Зарплата" NUMERIC(8, 2)) AS
$$
BEGIN
    RETURN QUERY
        SELECT t.type, t.seeker_day, t.salary
        FROM
        (
            SELECT *
            FROM applications a
            LEFT JOIN employment_types e
                ON e.id = a.employment_type_id
            LEFT JOIN positions p
                ON p.id = a.position_id
            WHERE experience IS NULL AND
                  lower(p.position) = lower(a_position)
        ) t
        ORDER BY 1, 2, 3;
END;
$$ LANGUAGE 'plpgsql' STRICT;


/* ===== Lab7 ===== */


/* ===== 1 ===== */


CREATE OR REPLACE VIEW num_vacancies_from_each_employer AS
    SELECT 
        e.employer "Компания",
        count(v.employer_id) "Количество вакансий"
    FROM employers e
    LEFT JOIN vacancies v
        ON e.id = v.employer_id
    GROUP BY e.employer
    ORDER BY 1, 2;


/* ===== 2 ===== */


CREATE OR REPLACE FUNCTION get_max_salaries_for_position(a_position VARCHAR(50))
    RETURNS TABLE(
        "Компания" citext, 
        "Максимальная зарплата" NUMERIC(8, 2)) AS
$$
BEGIN
    RETURN QUERY
        SELECT e.employer, max(v.salary_new)
        FROM employers e
        JOIN vacancies v
            ON e.id = v.employer_id
        JOIN positions p
            ON p.id = v.position_id
        WHERE lower(p.position) = lower(a_position)
        GROUP BY e.employer
        ORDER BY 1, 2;
END;
$$ LANGUAGE 'plpgsql' STRICT;


/* ===== 3 ===== */


CREATE OR REPLACE FUNCTION get_seekers_whose_total_experience_exceeds(a_experience INT)
    RETURNS TABLE(
        "Фамилия" VARCHAR(50), 
        "Имя" VARCHAR(50), 
        "Отчество" VARCHAR(50),
        "Суммарный опыт работы" NUMERIC(2, 0)) AS
$$
BEGIN
    RETURN QUERY
        SELECT last_name, first_name, patronymic, sum(a.experience)
        FROM seekers s
        JOIN applications a
            ON s.id = a.seeker_id
        GROUP BY s.id
        HAVING sum(a.experience) > a_experience
        ORDER BY 1, 2, 3, 4;
END;
$$ LANGUAGE 'plpgsql' STRICT;


/* ===== 4 ===== */


CREATE OR REPLACE FUNCTION 
    get_positions_from_open_vacancies_whose_average_salary_exceeds(a_salary NUMERIC(8, 2))
        RETURNS TABLE("Должность" citext, "Средняя зарплата" NUMERIC(8, 2)) AS
$$
BEGIN
    RETURN QUERY
        SELECT p.position, round(avg(v.salary_new), 0) avg
        FROM vacancies v
        JOIN positions p
            ON p.id = v.position_id
        WHERE v.vacancy_end IS FALSE
        GROUP BY p.position
        HAVING avg(v.salary_new) > a_salary
        ORDER BY p.position, avg;
END;
$$ LANGUAGE 'plpgsql' STRICT;


/* ===== 5 ===== */


CREATE OR REPLACE FUNCTION get_num_applications_for_each_employment_type()
    RETURNS TABLE("Тип занятости" citext, "Количество заявок" BIGINT) AS
$$
BEGIN
    RETURN QUERY
        SELECT t.emp_type, t.count
        FROM
        (
            SELECT e.type emp_type, count(a.employment_type_id) count
            FROM employment_types e
            JOIN applications a
                ON e.id = a.employment_type_id
            GROUP BY e.type
        ) t
        ORDER BY t.count, t.emp_type;
END;
$$ LANGUAGE 'plpgsql' STRICT;


/* ===== 6 ===== */


CREATE OR REPLACE FUNCTION 
    get_latest_vacancy_of_employers_whose_name_contains(a_pattern VARCHAR(40))
        RETURNS TABLE(
            "Компания" citext,
            "Дата размещения последней вакансии" DATE) AS
$$
BEGIN
    RETURN QUERY  
        SELECT employer, max(employer_day)::DATE v_date
        FROM employers e
        LEFT JOIN vacancies v 
            ON e.id = v.employer_id
        WHERE e.id IN
        (
            SELECT id
            FROM employers
            WHERE lower(employer) LIKE '%' || lower(a_pattern) || '%'
        )
        GROUP BY e.employer
        ORDER BY v_date, employer;
END;
$$ LANGUAGE 'plpgsql' STRICT;
