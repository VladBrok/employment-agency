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
DROP TABLE IF EXISTS addresses CASCADE;
DROP TABLE IF EXISTS streets CASCADE;
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

CREATE TABLE streets
(
    id SERIAL PRIMARY KEY,
    district_id INT NOT NULL,
    street citext UNIQUE NOT NULL,
    postal_code INT CHECK (postal_code > 0),
    FOREIGN KEY (district_id)
        REFERENCES districts (id)
        ON DELETE CASCADE
);

CREATE TABLE addresses
(
    id SERIAL PRIMARY KEY,
    street_id INT NOT NULL,
    building_number standart_building_number NOT NULL,
    FOREIGN KEY (street_id)
        REFERENCES streets (id)
        ON DELETE CASCADE
);

CREATE TABLE employers
(
    id SERIAL PRIMARY KEY,
    property_id INT NOT NULL,
    address_id INT NOT NULL,
    employer citext NOT NULL,
    phone phone_number NOT NULL,
    email email_address,
    FOREIGN KEY (property_id)
        REFERENCES properties (id)
        ON DELETE CASCADE,
    FOREIGN KEY (address_id)
        REFERENCES addresses (id)
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
    address_id INT NOT NULL,
    speciality_id INT NOT NULL,
    last_name VARCHAR(20) NOT NULL,
    first_name VARCHAR(20) NOT NULL,
    patronymic VARCHAR(20),
    phone phone_number,
    birthday DATE NOT NULL CHECK ((DATE_PART('year', now()::date) - DATE_PART('year', birthday::date)) > 15),
    registration_city VARCHAR(20) NOT NULL,
    recommended BOOLEAN NOT NULL,
    pol BOOLEAN NOT NULL,
    education VARCHAR(50),
    FOREIGN KEY (status_id)
        REFERENCES statuses (id)
        ON DELETE CASCADE,
    FOREIGN KEY (address_id)
        REFERENCES addresses (id)
        ON DELETE CASCADE,
    FOREIGN KEY (speciality_id)
        REFERENCES positions (id)
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
    
INSERT INTO streets(district_id, street, postal_code)
VALUES
    (1, 'Артема', 12000),
    (1, 'Университетская', 12000),
    (2, 'Олимпиева', 7032),
    (2, 'Рослого', 7064),
    (3, 'Терешковой', 8039),
    (3, 'Коммунаров', 5006),
    (4, 'Бальзака', 6032),
    (4, 'Майская', 7064),
    (5, 'Малышева', 7032),
    (5, 'Клубная', 12000),
    (6, 'Женевская', 3200),
    (6, 'Ермакова', 7064),
    (7, 'Чернова', 3200),
    (7, 'Овражная', 5006),
    (8, 'Озерная', 7031),
    (8, 'Литературная', 3200),
    (9, 'Щегловская', 12000),
    (9, 'Юбилейная', 5006);


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


CREATE OR REPLACE FUNCTION populate_addresses(count INT)
    RETURNS INT AS
$$
DECLARE
    num_streets INT := (SELECT count(*) FROM streets);
BEGIN
    FOR i IN 1..count LOOP
        INSERT INTO addresses(street_id, building_number)
           VALUES(random_between(1, num_streets), i);
    END LOOP;
    RETURN NULL;
END;
$$ LANGUAGE 'plpgsql' STRICT;


CREATE OR REPLACE FUNCTION populate_employers(count INT)
    RETURNS INT AS
$$
DECLARE
    num_properties INT := (SELECT count(*) FROM properties);
    num_addresses INT := (SELECT count(*) FROM addresses);
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
        INSERT INTO employers(property_id, address_id, employer, phone, email)
        VALUES 
        (
            random_between(1, num_properties),
            random_between(1, num_addresses),
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
    num_addresses INT := (SELECT count(*) FROM addresses);
    num_specialities INT := (SELECT count(*) FROM positions);
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
            random_between(1, num_addresses),
            random_between(1, num_specialities),
            last_names[random_between(1, array_length(last_names, 1))],
            first_names[random_between(1, array_length(first_names, 1))],
            patronymics[random_between(1, array_length(patronymics, 1))],
            random_phone(),
            random_date('1990-01-01 00:00:00', '2004-01-01 00:00:00')::DATE,
            cities[random_between(1, array_length(cities, 1))],
            random_bool(),
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
            CASE WHEN random() > 0.6 THEN random_between(1, 10) ELSE NULL END
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
    PERFORM populate_addresses(count);
    PERFORM populate_employers(count);
    PERFORM populate_vacancies(count);
    PERFORM populate_seekers(count);
    PERFORM populate_applications(count);
    RETURN NULL;
END;
$$ LANGUAGE 'plpgsql' STRICT;

/* == Uncomment to populate all tables == */
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

/*SELECT * FROM  streets ;
SELECT * FROM  districts ;
SELECT * FROM  properties ;
SELECT * FROM  employers ;
SELECT * FROM  vacancies ;
SELECT * FROM  statuses ;
SELECT * FROM  addresses ;
SELECT * FROM  seekers ;
SELECT * FROM  positions ;
SELECT * FROM  employment_types ;
SELECT * FROM  applications ;*/
