class Column {
  constructor(realName, displayName, convertValue) {
    this.realName = realName;
    this.displayName = displayName;
    this.convertValue = convertValue ?? ((v) => v);
  }
}

const columnInfo = [
  new Column("table_name", "название таблицы"),
  new Column("operation", "операция"),
  new Column("time_modified", "время совершения операции"),
  new Column("property", "тип собственности"),
  new Column("position", "должность"),
  new Column("status", "социальный статус"),
  new Column("type", "тип занятости"),
  new Column("district", "район"),
  new Column("street", "улица"),
  new Column("postal_code", "почтовый индекс"),
  new Column("building_number", "номер дома"),
  new Column("employer", "работодатель"),
  new Column("phone", "номер телефона"),
  new Column("email", "почта"),
  new Column("employer_day", "дата размещения вакансии"),
  new Column("salary_new", "зарплата"),
  new Column("chart_new", "график работы"),
  new Column("vacancy_end", "вакансия закрыта", (v) =>
    v === "True" ? "да" : "нет"
  ),
  new Column("last_name", "фамилия"),
  new Column("first_name", "имя"),
  new Column("patronymic", "отчество"),
  new Column("birthday", "день рождения", (v) =>
    v.substring(0, v.indexOf("00:") - 3)
  ),
  new Column("registration_city", "город регистрации"),
  new Column("recommended", "рекомендован", (v) =>
    v === "True" ? "да" : "нет"
  ),
  new Column("pol", "пол", (v) => (v === "True" ? "мужской" : "женский")),
  new Column("education", "образование"),
  new Column("seeker_day", "дата публикации"),
  new Column("information", "информация"),
  new Column("photo", "фото"),
  new Column("salary", "зарплата"),
  new Column("experience", "опыт работы", (v) => (v === "" ? "Нет" : v)),
];

const columns = {};
for (const info of columnInfo) {
  columns[info.realName] = info;
  columns[info.displayName] = info;
}

export default columns;
