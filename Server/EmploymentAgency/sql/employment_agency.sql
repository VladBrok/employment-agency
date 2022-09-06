DROP TRIGGER IF EXISTS applicatons_set_seeker_day ON applications CASCADE;
DROP TRIGGER IF EXISTS vacancies_set_employer_day ON vacancies CASCADE;
DROP TRIGGER IF EXISTS employer_log_delete ON employers CASCADE;
DROP TRIGGER IF EXISTS seeker_log_delete ON seekers CASCADE;
DROP TRIGGER IF EXISTS employer_log_update ON employers CASCADE;
DROP TRIGGER IF EXISTS seeker_log_update ON seekers CASCADE;

DROP TABLE IF EXISTS change_log CASCADE;
DROP TABLE IF EXISTS seekers CASCADE;
DROP TABLE IF EXISTS employers CASCADE;
DROP TABLE IF EXISTS vacancies CASCADE;
DROP TABLE IF EXISTS applications CASCADE;
DROP TABLE IF EXISTS districts CASCADE;
DROP TABLE IF EXISTS employment_types CASCADE;
DROP TABLE IF EXISTS statuses CASCADE;
DROP TABLE IF EXISTS positions CASCADE;
DROP TABLE IF EXISTS properties CASCADE;

DROP DOMAIN IF EXISTS email_address CASCADE;
DROP DOMAIN IF EXISTS phone_number CASCADE;
DROP DOMAIN IF EXISTS standart_building_number CASCADE;

DROP INDEX IF EXISTS applications_seeker_day CASCADE;
DROP INDEX IF EXISTS applications_experience CASCADE;
DROP INDEX IF EXISTS applications_salary CASCADE;
DROP INDEX IF EXISTS vacancies_salary_new CASCADE;
DROP INDEX IF EXISTS vacancies_employer_day CASCADE;
DROP INDEX IF EXISTS district_id CASCADE;
DROP INDEX IF EXISTS property_id CASCADE;
DROP INDEX IF EXISTS employer_id CASCADE;
DROP INDEX IF EXISTS position_id CASCADE;
DROP INDEX IF EXISTS status_id CASCADE;
DROP INDEX IF EXISTS seeker_id CASCADE;
DROP INDEX IF EXISTS position_id CASCADE;
DROP INDEX IF EXISTS employment_type_id CASCADE;

CREATE EXTENSION IF NOT EXISTS citext;

CREATE DOMAIN email_address AS VARCHAR(50)
CHECK (VALUE ~* '^[a-z0-9.+-_]+@[a-z0-9]+[.][a-z]+$');

CREATE DOMAIN phone_number AS VARCHAR(10)
CHECK (VALUE ~ '^071\d{7}');

CREATE DOMAIN standart_building_number AS INT
CHECK (VALUE > 0);


CREATE TABLE change_log
(
    id SERIAL PRIMARY KEY,
    table_name VARCHAR(50) NOT NULL,
    operation VARCHAR(10) NOT NULL,
    record_id INT NOT NULL,
    time_modified TIMESTAMP NOT NULL DEFAULT now(),
    user_modified NAME NOT NULL
);

CREATE TABLE properties
(
    id SERIAL PRIMARY KEY,
    property citext UNIQUE NOT NULL
);

CREATE TABLE positions
(
    id SERIAL PRIMARY KEY,
    position citext UNIQUE NOT NULL
);

CREATE TABLE statuses
(
    id SERIAL PRIMARY KEY,
    status citext UNIQUE NOT NULL CHECK (length(status) > 0)
);

CREATE TABLE employment_types
(
    id SERIAL PRIMARY KEY,
    type citext UNIQUE NOT NULL
);

CREATE TABLE districts
(
    id SERIAL PRIMARY KEY,
    district citext UNIQUE NOT NULL
);

CREATE TABLE employers
(
    id SERIAL PRIMARY KEY,
    property_id INT NOT NULL,
    district_id INT NOT NULL,
    employer citext NOT NULL,
    phone phone_number NOT NULL,
    email email_address,
    FOREIGN KEY (property_id)
        REFERENCES properties (id)
        ON DELETE CASCADE,
    FOREIGN KEY (district_id)
        REFERENCES districts (id)
        ON DELETE CASCADE
);

CREATE TABLE vacancies
(
    id SERIAL PRIMARY KEY,
    employer_id INT NOT NULL,
    position_id INT NOT NULL,
    employer_day TIMESTAMP NOT NULL DEFAULT now(),
    salary_new NUMERIC(8, 2),
    chart_new VARCHAR(50),
    vacancy_end BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (employer_id)
        REFERENCES employers (id)
        ON DELETE CASCADE,
    FOREIGN KEY (position_id)
        REFERENCES positions (id)
        ON DELETE CASCADE
);


CREATE TABLE seekers
(
    id SERIAL PRIMARY KEY,
    status_id INT NOT NULL,
    district_id INT NOT NULL,
    last_name VARCHAR(20) NOT NULL,
    first_name VARCHAR(20) NOT NULL,
    patronymic VARCHAR(20),
    phone phone_number,
    birthday DATE NOT NULL CHECK ((DATE_PART('year', now()::date) - DATE_PART('year', birthday::date)) > 15),
    registration_city VARCHAR(20) NOT NULL,
    pol BOOLEAN NOT NULL,
    education VARCHAR(50),
    FOREIGN KEY (status_id)
        REFERENCES statuses (id)
        ON DELETE CASCADE,
    FOREIGN KEY (district_id)
        REFERENCES districts (id)
        ON DELETE CASCADE
);

CREATE TABLE applications
(
    id SERIAL PRIMARY KEY,
    seeker_id INT NOT NULL,
    position_id INT NOT NULL,
    employment_type_id INT,
    seeker_day TIMESTAMP NOT NULL DEFAULT now(),
    information TEXT,
    photo VARCHAR(100),
    salary NUMERIC(8, 2),
    experience NUMERIC(2,0) CHECK (experience < 70),
    recommended BOOLEAN NOT NULL,
    FOREIGN KEY (seeker_id)
        REFERENCES seekers (id)
        ON DELETE CASCADE,
    FOREIGN KEY (position_id)
        REFERENCES positions (id)
        ON DELETE CASCADE,
    FOREIGN KEY (employment_type_id)
        REFERENCES employment_types(id)
        ON DELETE CASCADE
);


CREATE INDEX applications_seeker_day ON applications(seeker_day);
CREATE INDEX applications_experience ON applications(experience);
CREATE INDEX applications_salary ON applications(salary);
CREATE INDEX vacancies_salary_new ON vacancies(salary_new);
CREATE INDEX vacancies_employer_day ON vacancies(employer_day);
CREATE INDEX employers_property_id ON employers(property_id);
CREATE INDEX employers_district_id ON employers(district_id);
CREATE INDEX vacancies_employer_id ON vacancies(employer_id);
CREATE INDEX vacancies_position_id ON vacancies(position_id);
CREATE INDEX seekers_status_id ON seekers(status_id);
CREATE INDEX seekers_district_id ON seekers(district_id);
CREATE INDEX applications_seeker_id ON applications(seeker_id);
CREATE INDEX applications_position_id ON applications(position_id);
CREATE INDEX applications_employment_type_id ON applications(employment_type_id);


INSERT INTO statuses(status)
VALUES 
    ('Пенсионер'),
    ('Служащий'),
    ('Предприниматель'),
    ('Учащийся'),
    ('Студент');

INSERT INTO employment_types(type)
VALUES
    ('Полная занятость'),
    ('Частичная занятость'),
    ('Проектная работа'),
    ('Стажировка'),
    ('Волонтерство');

INSERT INTO properties(property)
VALUES
    ('Государственная'),
    ('Муниципальная'),
    ('Частная');

INSERT INTO positions(position)
VALUES
    ('Автослесарь'),
    ('Переводчик'),
    ('Специалист службы безопасности'),
    ('Менеджер по закупкам'),
    ('Продюсер'),
    ('Фотограф'),
    ('Учитель математики'),
    ('Психолог'),
    ('Инструктор тренажерного зала'),
    ('Разнорабочий'),
    ('Промоутер'),
    ('Агроном'),
    ('Страховой агент'),
    ('Курьер'),
    ('Экономист'),
    ('Юрисконсульт'),
    ('Медицинская сестра'),
    ('Ветеринарный врач'),
    ('Тестировщик'),
    ('Геодезист'),
    ('Диджей'),
    ('Мастер депиляции'),
    ('Оператор техподдержки'),
    ('Диктор');

INSERT INTO districts(district)
VALUES
    ('Ворошиловский'),
    ('Ленинский'),
    ('Кировский'),
    ('Буденновский'),
    ('Петровский'),
    ('Пролетарский'),
    ('Куйбышевский'),
    ('Киевский'),
    ('Калининский');
    

CREATE OR REPLACE FUNCTION random_between(low INT, high INT) 
   RETURNS INT AS
$$
BEGIN
   RETURN floor(random() * (high - low + 1) + low);
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION random_string(length INT)
    RETURNS TEXT AS
$$
DECLARE
    result TEXT := '';
    chars TEXT[] := '{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z}';
BEGIN
    FOR i IN 1..length LOOP
        result = result || chars[random_between(1, array_length(chars, 1))];
    END LOOP;
    RETURN result;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION random_date(min TIMESTAMP, max TIMESTAMP) 
   RETURNS TIMESTAMP AS
$$
BEGIN
   RETURN min + random() * (max - min);
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION random_phone() 
   RETURNS TEXT AS
$$
BEGIN
   RETURN '071' || to_char(random_between(0, 9999999), 'fm0000000');
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION random_bool() 
   RETURNS BOOLEAN AS
$$
BEGIN
   RETURN random_between(1, 10) % 2 = 0;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION populate_employers(count INT)
    RETURNS INT AS
$$
DECLARE
    num_properties INT := (SELECT count(*) FROM properties);
    num_districts INT := (SELECT count(*) FROM districts);
    alphabet TEXT[] := '{A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z}';
    employers_list TEXT[] := ARRAY(
                                 SELECT a || b || c || d
                                 FROM unnest(alphabet) a 
                                 CROSS JOIN unnest(alphabet) b
                                 CROSS JOIN unnest(alphabet) c
                                 CROSS JOIN unnest(alphabet) d);
    email_endings TEXT[] := '{.com, .ru, .su}';
BEGIN
    FOR i IN 1..count LOOP
        INSERT INTO employers(property_id, district_id, employer, phone, email)
        VALUES 
        (
            random_between(1, num_properties),
            random_between(1, num_districts),
            employers_list[i],
            random_phone(),
            random_string(10) || '@' || random_string(5) || email_endings[random_between(1, array_length(email_endings, 1))]
        );
    END LOOP;
    RETURN NULL;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION populate_vacancies(count INT)
    RETURNS INT AS
$$
DECLARE
    num_employers INT := (SELECT count(*) FROM employers);
    num_positions INT := (SELECT count(*) FROM positions);
    charts TEXT[] := '{гибкий, посменный, понедельник-пятница с 8 до 1,
                       вторник, четверг, суббота с 9 до 15, суммарный учет рабочего времени,
                       3 через 3, вахтовый график}';
BEGIN
    FOR i IN 1..count LOOP
        INSERT INTO vacancies(employer_id, position_id, employer_day, salary_new, chart_new, vacancy_end)
        VALUES
        (
            random_between(1, num_employers),
            random_between(1, num_positions),
            random_date('2016-01-01 00:00:00', '2022-01-01 00:00:00'),
            random_between(1, 100) * 1000,
            charts[random_between(1, array_length(charts, 1))],
            random_bool()
        );
    END LOOP;
    RETURN NULL;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION populate_seekers(count INT)
    RETURNS INT AS
$$
DECLARE
    num_statuses INT := (SELECT count(*) FROM statuses);
    num_districts INT := (SELECT count(*) FROM districts);
    first_names TEXT[] := '{Владимир, Николай, Никита, Федор, Альберто, Геннадий, 
                            Даниил, Тихон, Майкл, Георгий, Сергей, Ярослав, Алексей, Александр, Ростислав, Борис, Антонио,
                            Орландо, Хитоми, Ли}';
    last_names TEXT[] := '{Петров, Сидоров, Никифоров, Никулин, Павлов, Тихонов, Петренко,
                           Казатченко, Кулич, Джоржан, Такахаси, Судзуки, Ода, Коломбо, 
                           Риччи, Романо, Моретти, Коваленко, Ткаченко, Мельник}';
    patronymics TEXT[] := '{Николаевич, Петрович, Сидорович, Владимирович, Никифорович, 
                            Альбертович, Тихонович, Георгиевич, Ростиславович,
                            Михайлович, Романович, Олегович, Неонович, Кадзамович, 
                            Ярославович, Юриевич, Юсупович}';
    cities TEXT[] := '{Донецк, Москва, Борисполь, Нью-Йорк, Амстердам, Вена, Люксембург, Ереван}';
    educations TEXT[] := '{Донецкий национальный технический университет, Оксфордский университет, 
                           Калифорнийский технологический институт, Чикагский университет,
                           Имперский колледж Лондона, Университет Торонто, Йоркский Университет, 
                           Датский технический университет, Университетский коллежд Лондона}';
BEGIN
    FOR i IN 1..count LOOP
        INSERT INTO seekers
        VALUES
        (
            DEFAULT,
            random_between(1, num_statuses),
            random_between(1, num_districts),
            last_names[random_between(1, array_length(last_names, 1))],
            first_names[random_between(1, array_length(first_names, 1))],
            patronymics[random_between(1, array_length(patronymics, 1))],
            random_phone(),
            random_date('1990-01-01 00:00:00', '2004-01-01 00:00:00')::DATE,
            cities[random_between(1, array_length(cities, 1))],
            TRUE,
            educations[random_between(1, array_length(educations, 1))]
        );
    END LOOP;
    RETURN NULL;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION populate_applications(count INT)
    RETURNS INT AS
$$
DECLARE
    num_seekers INT := (SELECT count(*) FROM seekers);
    num_positions INT := (SELECT count(*) FROM positions);
    num_employment_types INT := (SELECT count(*) FROM employment_types);
BEGIN
    FOR i IN 1..count LOOP
        INSERT INTO applications
        VALUES
        (
            DEFAULT,
            random_between(1, num_seekers),
            random_between(1, num_positions),
            random_between(1, num_employment_types),
            random_date('2015-01-01 00:00:00', '2022-01-01 00:00:00'),
            md5(random()::TEXT),
            'sample.png',
            random_between(1, 100) * 1000,
            CASE WHEN random() > 0.6 THEN random_between(1, 10) ELSE NULL END,
            random_bool()
        );
    END LOOP;
    RETURN NULL;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION populate_all()
    RETURNS INT AS
$$
DECLARE
    count INT := 10000;
BEGIN
    PERFORM populate_employers(count);
    PERFORM populate_vacancies(count);
    PERFORM populate_seekers(count);
    PERFORM populate_applications(count);
    RETURN NULL;
END;
$$ LANGUAGE 'plpgsql' STRICT;


SELECT * FROM populate_all(); 


CREATE OR REPLACE FUNCTION set_seeker_day()
    RETURNS TRIGGER AS
$$
BEGIN
    NEW.seeker_day = now();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION set_employer_day()
    RETURNS TRIGGER AS
$$
BEGIN
    NEW.employer_day = now();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION write_log()
    RETURNS TRIGGER AS
$$
DECLARE
    operation VARCHAR(10) := TG_ARGV[0];
BEGIN
    INSERT INTO change_log
    VALUES(DEFAULT, TG_TABLE_NAME::REGCLASS::TEXT, operation, OLD.id, DEFAULT, SESSION_USER);
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER applicatons_set_seeker_day
    BEFORE INSERT ON applications
    FOR EACH ROW
        EXECUTE PROCEDURE set_seeker_day();

CREATE TRIGGER vacancies_set_employer_day
    BEFORE INSERT ON vacancies
    FOR EACH ROW
        EXECUTE PROCEDURE set_employer_day();

CREATE TRIGGER employer_log_delete 
    AFTER DELETE ON employers
    FOR EACH ROW
        EXECUTE PROCEDURE write_log('delete');

CREATE TRIGGER seeker_log_delete 
    AFTER DELETE ON seekers
    FOR EACH ROW
        EXECUTE PROCEDURE write_log('delete');
 
CREATE TRIGGER employer_log_update
    AFTER UPDATE ON employers
    FOR EACH ROW
        EXECUTE PROCEDURE write_log('update');

CREATE TRIGGER seeker_log_update 
    AFTER UPDATE ON seekers
    FOR EACH ROW
        EXECUTE PROCEDURE write_log('update');



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
        "Образование" VARCHAR(50)) AS
$$
BEGIN
    RETURN QUERY
        SELECT 
            last_name, 
            first_name, 
            patronymic,
            birthday, 
            education
        FROM seekers s
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
