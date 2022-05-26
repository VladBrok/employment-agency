import { fetchAllJson } from "./api.js";

class Column {
  constructor(realName, displayName, convertToInput, convertValue) {
    this.realName = realName;
    this.displayName = displayName;
    this.convertToInput = convertToInput;
    this.convertValue = convertValue ?? ((v) => v);
  }

  async makeSelect(id, endpoint, value) {
    const options = await fetchAllJson(endpoint);

    return `<select class="select" id=${id}>${options
      .map(
        (o) =>
          `<option ${
            o[this.realName].toLowerCase() === value?.trim().toLowerCase()
              ? "selected"
              : ""
          }>${o[this.realName]}</option>`
      )
      .join("")}</select>`;
  }

  makeRadio(value, ...options) {
    const name = `${this.realName}-name`;

    return `<div>${options
      .map(
        (o, i) => `
          <div class="radio-wrapper">
            <label for="${o}${i}">${o}</label>
            <input type="radio" name="${name}" class="radio-input" ${
          o.toLowerCase() === value?.trim().toLowerCase() ? "checked" : ""
        }>
          </div>`
      )
      .join("")}</div>`;
  }
}

const columnInfo = [
  new Column("table_name", "название таблицы"),
  new Column("operation", "операция"),
  new Column("time_modified", "время совершения операции"),
  new Column("property", "тип собственности", async function (id, value) {
    return await this.makeSelect(id, "/properties", value);
  }),
  new Column("position", "должность", async function (id, value) {
    return await this.makeSelect(id, "/positions", value);
  }),
  new Column("status", "социальный статус", async function (id, value) {
    return await this.makeSelect(id, "/statuses", value);
  }),
  new Column("type", "тип занятости", async function (id, value) {
    return await this.makeSelect(id, "/employment_types", value);
  }),
  new Column("district", "район", async function (id, value) {
    return await this.makeSelect(id, "/districts", value);
  }),
  new Column("street", "улица", async function (id, value) {
    return await this.makeSelect(id, "/streets", value);
  }),
  new Column("postal_code", "почтовый индекс", (id, value) =>
    makeNumberInput(id, value, 1, 10000)
  ),
  new Column("building_number", "номер дома", (id, value) =>
    makeNumberInput(id, value, 1, 1000000, true)
  ),
  new Column("employer", "работодатель", (id, value) =>
    makeTextInput(id, value, 1, 30, true)
  ),
  new Column("phone", "номер телефона", (id, value) =>
    makeTextInput(
      id,
      value,
      null,
      null,
      false,
      "071[0-9]{7}",
      "Пример: 0710120500"
    )
  ),
  new Column(
    "email",
    "почта",
    (id, value) => makeInput(id, value, "email").outerHTML
  ),
  new Column("employer_day", "дата размещения", (id, value) =>
    makeDateTimeInput(id, value, true)
  ),
  new Column("salary_new", "зарплата", (id, value) =>
    makeNumberInput(id, value, 1000, 1000000)
  ),
  new Column("chart_new", "график работы", (id, value) =>
    makeTextInput(id, value, 1, 30)
  ),
  new Column(
    "vacancy_end",
    "вакансия закрыта",
    function (_, value) {
      return this.makeRadio(value, "Да", "Нет");
    },
    (v) => (v === "True" ? "да" : "нет")
  ),
  new Column("last_name", "фамилия", (id, value) =>
    makeTextInput(id, value, 3, 20, true)
  ),
  new Column("first_name", "имя", (id, value) =>
    makeTextInput(id, value, 3, 20, true)
  ),
  new Column("patronymic", "отчество", (id, value) =>
    makeTextInput(id, value, 3, 20)
  ),
  new Column(
    "birthday",
    "день рождения",
    (id, value) =>
      makeDateInput(
        id,
        value,
        "1900-01-01",
        `${new Date().getFullYear() - 16}-01-01`,
        true
      ),
    (v) => v.substring(0, v.indexOf("00:") - 3)
  ),
  new Column("registration_city", "город регистрации", (id, value) =>
    makeTextInput(id, value, 3, 20, true)
  ),
  new Column(
    "recommended",
    "рекомендован",
    function (_, value) {
      return this.makeRadio(value, "Да", "Нет");
    },
    (v) => (v === "True" ? "да" : "нет")
  ),
  new Column(
    "pol",
    "пол",
    function (_, value) {
      return this.makeRadio(value, "Мужской", "Женский");
    },
    (v) => (v === "True" ? "мужской" : "женский")
  ),
  new Column("education", "образование", (id, value) =>
    makeTextInput(id, value, 3, 20)
  ),
  new Column("seeker_day", "дата публикации", (id, value) =>
    makeDateTimeInput(id, value, true)
  ),
  new Column("information", "информация", (id, value) =>
    makeTextInput(id, value)
  ),
  new Column("photo", "фото", (id, value) =>
    makeFileInput(id, value, "image/png, image/jpeg")
  ),
  new Column("salary", "зарплата", (id, value) =>
    makeNumberInput(id, value, 1000, 1000000)
  ),
  new Column(
    "experience",
    "опыт работы",
    (id, value) => makeNumberInput(id, value, 0, 69),
    (v) => (v === "" ? "Нет" : v)
  ),
];

const columns = {};
for (const info of columnInfo) {
  columns[info.realName] = info;
  columns[info.displayName] = info;
}

function makeFileInput(id, value, accept, required = false) {
  const input = makeInput(id, value, "file", required);
  input.setAttribute("accept", accept);
  return input.outerHTML;
}

function makeDateTimeInput(id, value, required = false) {
  return makeInput(id, formatDateTime(value), "datetime-local", required)
    .outerHTML;
}

function makeDateInput(id, value, min, max, required = false) {
  return makeMinMaxInput(id, formatDate(value), "date", min, max, required);
}

function makeNumberInput(id, value, min, max, required = false) {
  const comma = value.indexOf(",");
  return makeMinMaxInput(
    id,
    comma === -1 ? value : value.slice(0, comma),
    "number",
    min,
    max,
    required
  );
}

function makeMinMaxInput(id, value, type, min, max, required = false) {
  const input = makeInput(id, value, type, required);
  input.setAttribute("min", min);
  input.setAttribute("max", max);
  return input.outerHTML;
}

function makeTextInput(
  id,
  value,
  minlength = null,
  maxlength = null,
  required = false,
  pattern = null,
  placeholer = null
) {
  const input = makeInput(id, value, "text", required);
  if (minlength) {
    input.setAttribute("minlength", minlength);
  }
  if (maxlength) {
    input.setAttribute("maxlength", maxlength);
  }
  if (pattern) {
    input.setAttribute("pattern", pattern);
  }
  if (placeholer) {
    input.setAttribute("placeholder", placeholer);
  }
  return input.outerHTML;
}

function makeInput(id, value, type, required = false) {
  const input = document.createElement("input");
  input.setAttribute("id", id);
  input.setAttribute("type", type);
  input.setAttribute("value", value);
  if (required) {
    input.setAttribute("required", "");
  }
  input.className = "input";
  return input;
}

function formatDate(date) {
  const dayMonthYear = date.split(".");
  return dayMonthYear.reverse().join("-");
}

function formatDateTime(value) {
  const dateTime = value.split(" ");
  return `${formatDate(dateTime[0])}T${dateTime[1].padStart(8, "0")}`;
}

export default columns;
