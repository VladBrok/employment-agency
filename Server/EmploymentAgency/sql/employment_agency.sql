DROP TRIGGER IF EXISTS applicatons_set_seeker_day ON applications CASCADE;
DROP TRIGGER IF EXISTS check_experience_trigger ON applications CASCADE;
DROP TRIGGER IF EXISTS vacancies_set_employer_day ON vacancies CASCADE;
DROP TRIGGER IF EXISTS employer_log_delete ON employers CASCADE;
DROP TRIGGER IF EXISTS seeker_log_delete ON seekers CASCADE;
DROP TRIGGER IF EXISTS employer_log_update ON employers CASCADE;
DROP TRIGGER IF EXISTS seeker_log_update ON seekers CASCADE;
DROP TRIGGER IF EXISTS before_insert_change_log_trigger ON change_log CASCADE;
DROP TRIGGER IF EXISTS before_insert_educations_trigger ON educations CASCADE;
DROP TRIGGER IF EXISTS before_insert_cities_trigger ON cities CASCADE;
DROP TRIGGER IF EXISTS before_insert_properties_trigger ON properties CASCADE;
DROP TRIGGER IF EXISTS before_insert_positions_trigger ON positions CASCADE;
DROP TRIGGER IF EXISTS before_insert_statuses_trigger ON statuses CASCADE;
DROP TRIGGER IF EXISTS before_insert_employment_types_trigger ON employment_types CASCADE;
DROP TRIGGER IF EXISTS before_insert_districts_trigger ON districts CASCADE;
DROP TRIGGER IF EXISTS before_insert_employers_trigger ON employers CASCADE;
DROP TRIGGER IF EXISTS before_insert_vacancies_trigger ON vacancies CASCADE;
DROP TRIGGER IF EXISTS before_insert_seekers_trigger ON seekers CASCADE;
DROP TRIGGER IF EXISTS before_insert_applications_trigger ON applications CASCADE;


DROP SEQUENCE IF EXISTS change_log_id_seq CASCADE;
DROP SEQUENCE IF EXISTS educations_id_seq CASCADE;
DROP SEQUENCE IF EXISTS cities_id_seq CASCADE;
DROP SEQUENCE IF EXISTS properties_id_seq CASCADE;
DROP SEQUENCE IF EXISTS positions_id_seq CASCADE;
DROP SEQUENCE IF EXISTS statuses_id_seq CASCADE;
DROP SEQUENCE IF EXISTS employment_types_id_seq CASCADE;
DROP SEQUENCE IF EXISTS districts_id_seq CASCADE;
DROP SEQUENCE IF EXISTS employers_id_seq CASCADE;
DROP SEQUENCE IF EXISTS vacancies_id_seq CASCADE;
DROP SEQUENCE IF EXISTS seekers_id_seq CASCADE;
DROP SEQUENCE IF EXISTS applications_id_seq CASCADE;

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
DROP TABLE IF EXISTS cities CASCADE;
DROP TABLE IF EXISTS educations CASCADE;

DROP DOMAIN IF EXISTS email_address CASCADE;
DROP DOMAIN IF EXISTS phone_number CASCADE;

DROP INDEX IF EXISTS employer_company CASCADE;
DROP INDEX IF EXISTS seekers_city_id CASCADE;
DROP INDEX IF EXISTS seekers_education_id CASCADE;
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
DROP INDEX IF EXISTS districts_city_id CASCADE;

DROP FUNCTION IF EXISTS before_insert_change_log;
DROP FUNCTION IF EXISTS before_insert_educations;
DROP FUNCTION IF EXISTS before_insert_cities;
DROP FUNCTION IF EXISTS before_insert_properties;
DROP FUNCTION IF EXISTS before_insert_positions;
DROP FUNCTION IF EXISTS before_insert_statuses;
DROP FUNCTION IF EXISTS before_insert_employment_types;
DROP FUNCTION IF EXISTS before_insert_districts;
DROP FUNCTION IF EXISTS before_insert_employers;
DROP FUNCTION IF EXISTS before_insert_vacancies;
DROP FUNCTION IF EXISTS before_insert_seekers;
DROP FUNCTION IF EXISTS before_insert_applications;
DROP FUNCTION IF EXISTS random_between;
DROP FUNCTION IF EXISTS random_string;
DROP FUNCTION IF EXISTS random_date;
DROP FUNCTION IF EXISTS random_phone;
DROP FUNCTION IF EXISTS random_bool;
DROP FUNCTION IF EXISTS populate_employers;
DROP FUNCTION IF EXISTS populate_vacancies;
DROP FUNCTION IF EXISTS populate_seekers;
DROP FUNCTION IF EXISTS populate_applications;
DROP FUNCTION IF EXISTS populate_all;
DROP FUNCTION IF EXISTS set_seeker_day;
DROP FUNCTION IF EXISTS set_employer_day;
DROP FUNCTION IF EXISTS write_log;
DROP FUNCTION IF EXISTS get_applications_percent_after;
DROP FUNCTION IF EXISTS get_applications_percent_by_positions_after;
DROP FUNCTION IF EXISTS get_application_count_by_positions;
DROP FUNCTION IF EXISTS get_application_count_of_seekers_whose_name_starts_with;
DROP FUNCTION IF EXISTS get_min_salary_of_employer_with_name;
DROP FUNCTION IF EXISTS get_applications_with_position;
DROP FUNCTION IF EXISTS get_seekers_not_registered_in;
DROP FUNCTION IF EXISTS get_salaries_in_comparison_with;
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

CREATE EXTENSION IF NOT EXISTS citext;

CREATE DOMAIN email_address AS VARCHAR(50)
CHECK (VALUE ~* '^.+?@.+$');

CREATE DOMAIN phone_number AS VARCHAR(10)
CHECK (VALUE ~ '^071\d{7}');


CREATE SEQUENCE change_log_id_seq;
CREATE SEQUENCE educations_id_seq;
CREATE SEQUENCE cities_id_seq;
CREATE SEQUENCE properties_id_seq;
CREATE SEQUENCE positions_id_seq;
CREATE SEQUENCE statuses_id_seq;
CREATE SEQUENCE employment_types_id_seq;
CREATE SEQUENCE districts_id_seq;
CREATE SEQUENCE employers_id_seq;
CREATE SEQUENCE vacancies_id_seq;
CREATE SEQUENCE seekers_id_seq;
CREATE SEQUENCE applications_id_seq;


CREATE TABLE change_log
(
    id INT NOT NULL PRIMARY KEY,
    table_name VARCHAR(50) NOT NULL,
    operation VARCHAR(10) NOT NULL,
    record_id INT NOT NULL,
    time_modified TIMESTAMP NOT NULL DEFAULT now(),
    user_modified NAME NOT NULL
);

CREATE TABLE educations
(
  id INT NOT NULL PRIMARY KEY,
  education citext UNIQUE NOT NULL
);

CREATE TABLE cities
(
  id INT NOT NULL PRIMARY KEY,
  city citext UNIQUE NOT NULL
);

CREATE TABLE properties
(
    id INT NOT NULL PRIMARY KEY,
    property citext UNIQUE NOT NULL
);

CREATE TABLE positions
(
    id INT NOT NULL PRIMARY KEY,
    position citext UNIQUE NOT NULL
);

CREATE TABLE statuses
(
    id INT NOT NULL PRIMARY KEY,
    status citext UNIQUE NOT NULL CHECK (length(status) > 0)
);

CREATE TABLE employment_types
(
    id INT NOT NULL PRIMARY KEY,
    type citext UNIQUE NOT NULL
);

CREATE TABLE districts
(
    id INT NOT NULL PRIMARY KEY,
    city_id INT NOT NULL,
    district citext UNIQUE NOT NULL,
    FOREIGN KEY (city_id)
        REFERENCES cities (id)
        ON DELETE CASCADE
);

CREATE TABLE employers
(
    id INT NOT NULL PRIMARY KEY,
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
    id INT NOT NULL PRIMARY KEY,
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
    id INT NOT NULL PRIMARY KEY,
    status_id INT NOT NULL,
    district_id INT NOT NULL,
    registration_city_id INT NOT NULL,
    education_id INT NOT NULL,
    last_name VARCHAR(20) NOT NULL,
    first_name VARCHAR(20) NOT NULL,
    patronymic VARCHAR(20),
    phone phone_number,
    birthday DATE NOT NULL CHECK (EXTRACT(YEAR FROM age(birthday))::NUMERIC > 15),
    pol BOOLEAN NOT NULL,
    FOREIGN KEY (status_id)
        REFERENCES statuses (id)
        ON DELETE CASCADE,
    FOREIGN KEY (district_id)
        REFERENCES districts (id)
        ON DELETE CASCADE,
    FOREIGN KEY (registration_city_id)
        REFERENCES cities (id)
        ON DELETE CASCADE,
    FOREIGN KEY (education_id)
        REFERENCES educations (id)
        ON DELETE CASCADE
);

CREATE TABLE applications
(
    id INT NOT NULL PRIMARY KEY,
    seeker_id INT NOT NULL,
    position_id INT NOT NULL,
    employment_type_id INT,
    seeker_day TIMESTAMP NOT NULL DEFAULT now(),
    information TEXT,
    photo VARCHAR(100),
    salary NUMERIC(8, 2),
    experience NUMERIC(2,0) CHECK (experience >= 0),
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
CREATE INDEX employer_company ON employers(employer);
CREATE INDEX vacancies_employer_id ON vacancies(employer_id);
CREATE INDEX vacancies_position_id ON vacancies(position_id);
CREATE INDEX seekers_status_id ON seekers(status_id);
CREATE INDEX seekers_district_id ON seekers(district_id);
CREATE INDEX seekers_education_id ON seekers(education_id);
CREATE INDEX applications_seeker_id ON applications(seeker_id);
CREATE INDEX applications_position_id ON applications(position_id);
CREATE INDEX applications_employment_type_id ON applications(employment_type_id);
CREATE INDEX districts_city_id ON districts(city_id);
CREATE INDEX seekers_city_id ON seekers(registration_city_id);


CREATE OR REPLACE FUNCTION check_experience() RETURNS TRIGGER AS $$
DECLARE
  max_experience INT;
  age INT;
BEGIN
  IF NEW.experience IS NULL THEN
    RETURN NEW;
  END IF;

  SELECT EXTRACT(YEAR FROM age(birthday))::NUMERIC 
  FROM seekers 
  WHERE id = NEW.seeker_id
  INTO age;
  max_experience := age - 16;

  IF NEW.experience > max_experience THEN
      RAISE EXCEPTION 'Опыт работы соискателя слишком велик';
  END IF;

  RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER check_experience_trigger
BEFORE INSERT OR UPDATE ON applications
FOR EACH ROW EXECUTE PROCEDURE check_experience();


CREATE OR REPLACE FUNCTION before_insert_change_log() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('change_log_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_change_log_trigger
BEFORE INSERT ON change_log
FOR EACH ROW EXECUTE PROCEDURE before_insert_change_log();


CREATE OR REPLACE FUNCTION before_insert_educations() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('educations_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_educations_trigger
BEFORE INSERT ON educations
FOR EACH ROW EXECUTE PROCEDURE before_insert_educations();


CREATE OR REPLACE FUNCTION before_insert_cities() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('cities_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_cities_trigger
BEFORE INSERT ON cities
FOR EACH ROW EXECUTE PROCEDURE before_insert_cities();


CREATE OR REPLACE FUNCTION before_insert_properties() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('properties_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_properties_trigger
BEFORE INSERT ON properties
FOR EACH ROW EXECUTE PROCEDURE before_insert_properties();


CREATE OR REPLACE FUNCTION before_insert_positions() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('positions_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_positions_trigger
BEFORE INSERT ON positions
FOR EACH ROW EXECUTE PROCEDURE before_insert_positions();


CREATE OR REPLACE FUNCTION before_insert_statuses() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('statuses_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_statuses_trigger
BEFORE INSERT ON statuses
FOR EACH ROW EXECUTE PROCEDURE before_insert_statuses();


CREATE OR REPLACE FUNCTION before_insert_employment_types() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('employment_types_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_employment_types_trigger
BEFORE INSERT ON employment_types
FOR EACH ROW EXECUTE PROCEDURE before_insert_employment_types();


CREATE OR REPLACE FUNCTION before_insert_districts() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('districts_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_districts_trigger
BEFORE INSERT ON districts
FOR EACH ROW EXECUTE PROCEDURE before_insert_districts();


CREATE OR REPLACE FUNCTION before_insert_employers() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('employers_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_employers_trigger
BEFORE INSERT ON employers
FOR EACH ROW EXECUTE PROCEDURE before_insert_employers();


CREATE OR REPLACE FUNCTION before_insert_vacancies() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('vacancies_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_vacancies_trigger
BEFORE INSERT ON vacancies
FOR EACH ROW EXECUTE PROCEDURE before_insert_vacancies();


CREATE OR REPLACE FUNCTION before_insert_seekers() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('seekers_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_seekers_trigger
BEFORE INSERT ON seekers
FOR EACH ROW EXECUTE PROCEDURE before_insert_seekers();


CREATE OR REPLACE FUNCTION before_insert_applications() RETURNS TRIGGER AS $$
BEGIN
    NEW.id = nextval('applications_id_seq');
    RETURN NEW;
END; $$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_applications_trigger
BEFORE INSERT ON applications
FOR EACH ROW EXECUTE PROCEDURE before_insert_applications();


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

INSERT INTO cities(city)
VALUES
    ('Донецк'),
    ('Москва'),
    ('Борисполь'),
    ('Амстердам');

INSERT INTO districts(city_id, district)
VALUES
    (1, 'Ворошиловский'),
    (1, 'Ленинский'),
    (2, 'Кировский'),
    (2, 'Буденновский'),
    (3, 'Петровский'),
    (4, 'Пролетарский'),
    (4, 'Куйбышевский'),
    (4, 'Киевский'),
    (4, 'Калининский');
    
INSERT INTO educations(education)
VALUES
    ('Донецкий национальный технический университет'),
    ('Оксфордский университет'),
    ('Калифорнийский технологический институт'),
    ('Чикагский университет'),
    ('Имперский колледж Лондона'),
    ('Университет Торонто'),
    ('Йоркский Университет'),
    ('Датский технический университет'),
    ('Университетский коллежд Лондона');


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
    num_cities INT := (SELECT count(*) FROM cities);
    num_educations INT := (SELECT count(*) FROM educations);
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
BEGIN
    FOR i IN 1..count LOOP
        INSERT INTO seekers
        VALUES
        (
            DEFAULT,
            random_between(1, num_statuses),
            random_between(1, num_districts),
            random_between(1, num_cities),
            random_between(1, num_educations),
            last_names[random_between(1, array_length(last_names, 1))],
            first_names[random_between(1, array_length(first_names, 1))],
            patronymics[random_between(1, array_length(patronymics, 1))],
            random_phone(),
            random_date('1970-01-01 00:00:00', '1990-01-01 00:00:00')::DATE,
            TRUE
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
DROP VIEW IF EXISTS total_vacancies_including_not_ended;

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
        

-- select '-----По всему агентству-----', null
-- union all
-- (select null, * from get_applications_percent_after(2016))
-- union all
-- (select '-----По должностям-----', null)
-- union all
-- (select * from get_applications_percent_by_positions_after(2016))

/* == NEW == */

CREATE OR REPLACE VIEW total_vacancies_including_not_ended AS
    SELECT e.employer "Компания",
           COUNT(*) "Всего вакансий", 
           COUNT(CASE WHEN NOT v.vacancy_end THEN v.vacancy_end END) "Открытых вакансий"
    FROM vacancies v
    JOIN employers e
        ON e.id = v.employer_id
	GROUP BY e.employer
	ORDER BY 1, 2, 3;


CREATE OR REPLACE FUNCTION get_application_count_of_seekers_whose_name_starts_with(a_chars varchar(20))
RETURNS TABLE ("Имя" varchar(20), 
			"Фамилия" varchar(20), 
			"Отчество" varchar(20), 
			"Всего заявок" bigint)
AS $$ BEGIN
	RETURN QUERY 
	    SELECT s.first_name, s.last_name, s.patronymic, count(*) 
        FROM applications a
        JOIN seekers s
            ON s.id = a.seeker_id
        WHERE lower(s.first_name) LIKE concat(a_chars, '%')
        GROUP BY s.id
        ORDER BY 1, 2, 3, 4;
END; $$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION get_min_salary_of_employer_with_name(a_name varchar(100))
RETURNS TABLE ("Компания" citext, 
			"Минимальная зарплата" numeric, 
			"Почта" email_address, 
			"Телефон" phone_number)
AS $$ BEGIN
	RETURN QUERY 
	    SELECT e.employer, min(v.salary_new), e.email, e.phone
        FROM vacancies v
        JOIN employers e 
            ON e.id = v.employer_id
        WHERE lower(e.employer) = lower(a_name)
        GROUP BY e.id
        ORDER BY 1, 2, 3, 4;
END; $$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE VIEW num_of_seekers_with_university_education AS
    SELECT e.education "Образование", count(*) "Количество соискателей"
    FROM seekers s
    JOIN educations e
        ON s.education_id = e.id
    WHERE e.education LIKE '%университет%'
    GROUP BY e.id
    ORDER BY 1, 2;


CREATE OR REPLACE VIEW employers_all_and_by_districts AS
    (SELECT '-----По всему агентству-----' "Район", null "Количество работодателей")
        UNION ALL
    	
    (SELECT null "Район", COUNT(*) "Количество работодателей"
     FROM employers)
        UNION ALL
    	
    (SELECT '-----По районам-----' "Район", null "Количество работодателей")
        UNION ALL
    	
    (SELECT d.district "Район", count(*) "Количество работодателей"
     FROM employers e
     JOIN districts d
         ON d.id = e.district_id
    GROUP BY d.district
    ORDER BY 1, 2);


CREATE OR REPLACE FUNCTION get_applications_with_position(a_position varchar(100))
RETURNS TABLE ("Дата подачи заявления" timestamp, 
			"Зарплата" numeric(8, 2), 
			"Опыт" numeric(2), 
			"Рекомендован" boolean)
AS $$ BEGIN
	RETURN QUERY 	    
		SELECT seeker_day, salary, experience, recommended
		FROM applications
		WHERE position_id IN (
    		SELECT id
			FROM positions
			WHERE lower(position) = lower(a_position)
        )
        ORDER BY 1, 2, 3, 4;
END; $$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION get_seekers_not_registered_in(a_city varchar(100))
RETURNS TABLE ("Имя" varchar(20), 
			"Фамилия" varchar(20), 
			"Отчество" varchar(20),
			"Телефон" phone_number) 
AS $$ BEGIN
	RETURN QUERY 	    
		SELECT first_name, last_name, patronymic, phone
		FROM seekers s
		WHERE s.registration_city_id NOT IN (
			SELECT c.id
			FROM cities c
			WHERE lower(c.city) = lower(a_city)
		)
        ORDER BY 1, 2, 3, 4;
END; $$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION get_salaries_in_comparison_with(a_salary numeric(8, 2))
RETURNS TABLE ("Дата размещения вакансии" timestamp, 
			"График" varchar(50), 
			"Зарплата" text) 
AS $$ BEGIN
	RETURN QUERY 	    		
      SELECT 
          v.employer_day,
          v.chart_new,
      	  CASE 
              WHEN v.salary_new > a_salary THEN 'больше ' || a_salary
              WHEN v.salary_new < a_salary THEN 'меньше ' || a_salary
          ELSE 'равно ' || a_salary END
      FROM vacancies v
      WHERE v.employer_id IN (
      	SELECT e.id
      	FROM employers e
      	JOIN properties p
      	    ON p.id = e.property_id
      	WHERE p.property = 'Частная'
      )
      ORDER BY 1, 2, 3;
END; $$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE VIEW seekers_with_even_average_experience AS
  SELECT 
      s.last_name "Фамилия",
      s.first_name "Имя",
      s.patronymic "Отчество",
      round(avg(ap.experience), 0) "Средний опыт"
  FROM seekers s
  JOIN applications ap
      ON ap.seeker_id = s.id
  WHERE s.id IN (
  	SELECT a.seeker_id
  	FROM applications a
  	GROUP BY a.seeker_id
  	HAVING avg(a.experience) % 2 = 0
  )
  GROUP BY s.id
  ORDER BY 1, 2, 3, 4;



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
        JOIN districts d
            ON d.id = s.district_id
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
        "Образование" citext) AS
$$
BEGIN
    RETURN QUERY
        SELECT 
            last_name, 
            first_name, 
            patronymic,
            birthday, 
            e.education
        FROM seekers s
		JOIN educations e
		  ON s.education_id = e.id
        WHERE s.birthday > a_date
        ORDER BY 1, 2, 3, 4, 5;
END;
$$ LANGUAGE 'plpgsql' STRICT;


/* ===== 2 ===== */


CREATE OR REPLACE VIEW employer_addresses AS
    SELECT 
        e.employer "Компания", 
        d.district "Район", 
		c.city "Город"
    FROM employers e
    JOIN districts d
        ON e.district_id = d.id
	JOIN cities c
	    ON d.city_id = c.id
    ORDER BY 1, 2, 3;



CREATE OR REPLACE VIEW employment_types_and_salaries AS
    SELECT DISTINCT
        e.type "Тип занятости",
        round(avg(a.salary)::numeric, 0) "Средняя зарплата",
        round(avg(a.experience)::numeric, 0) "Средний опыт"
    FROM applications a
    JOIN employment_types e
        ON a.employment_type_id = e.id
    GROUP BY e.type
    ORDER BY 1, 2, 3;


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
        c.city "Город регистрации",
        seeker_day "Дата размещения заявки",
        salary "Ожидаемая зарплата"
    FROM applications a
    RIGHT JOIN seekers s
        ON s.id = a.seeker_id
    JOIN cities c
	    ON c.id = s.registration_city_id
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
