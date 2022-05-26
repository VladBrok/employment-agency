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
  new Column("postal_code", "почтовый индекс", (id) =>
    makeNumberInput(id, 1, 10000)
  ),
  new Column("building_number", "номер дома", (id) =>
    makeNumberInput(id, 1, 1000000, true)
  ),
  new Column("employer", "работодатель", (id) =>
    makeTextInput(id, 1, 30, true)
  ),
  new Column("phone", "номер телефона", (id) =>
    makeTextInput(id, null, null, false, "071[0-9]{7}", "Пример: 0710120500")
  ),
  new Column("email", "почта", (id) => makeInput(id, "email").outerHTML),
  new Column(
    "employer_day",
    "дата размещения",
    (id) => makeInput(id, "datetime-local", true).outerHTML
  ),
  new Column("salary_new", "зарплата", (id) =>
    makeNumberInput(id, 10000, 1000000)
  ),
  new Column("chart_new", "график работы", (id) => makeTextInput(id, 1, 30)),
  new Column(
    "vacancy_end",
    "вакансия закрыта",
    function (_, value) {
      return this.makeRadio(value, "Да", "Нет");
    },
    (v) => (v === "True" ? "да" : "нет")
  ),
  new Column("last_name", "фамилия", (id) => makeTextInput(id, 3, 20, true)),
  new Column("first_name", "имя", (id) => makeTextInput(id, 3, 20, true)),
  new Column("patronymic", "отчество", (id) => makeTextInput(id, 3, 20)),
  new Column(
    "birthday",
    "день рождения",
    (id) =>
      makeDateInput(
        id,
        "1900-01-01",
        `${new Date().getFullYear() - 16}-01-01`,
        true
      ),
    (v) => v.substring(0, v.indexOf("00:") - 3)
  ),
  new Column("registration_city", "город регистрации", (id) =>
    makeTextInput(id, 3, 20, true)
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
  new Column("education", "образование", (id) => makeTextInput(id, 3, 20)),
  new Column("seeker_day", "дата публикации", (id) =>
    makeInput(id, "datetime-local", true)
  ),
  new Column("information", "информация", (id) => makeTextInput(id)),
  new Column("photo", "фото", (id) =>
    makeFileInput(id, "image/png, image/jpeg")
  ),
  new Column("salary", "зарплата", (id) => makeNumberInput(id, 10000, 1000000)),
  new Column(
    "experience",
    "опыт работы",
    (id) => makeNumberInput(id, 0, 69),
    (v) => (v === "" ? "Нет" : v)
  ),
];

const columns = {};
for (const info of columnInfo) {
  columns[info.realName] = info;
  columns[info.displayName] = info;
}

function makeFileInput(id, accept, required = false) {
  const input = makeInput(id, "file", required);
  input.setAttribute("accept", accept);
  return input.outerHTML;
}

function makeDateInput(id, min, max, required = false) {
  makeMinMaxInput(id, "date", min, max, required);
}

function makeNumberInput(id, min, max, required = false) {
  return makeMinMaxInput(id, "number", min, max, required);
}

function makeMinMaxInput(id, type, min, max, required = false) {
  const input = makeInput(id, type, required);
  input.setAttribute("min", min);
  input.setAttribute("max", max);
  return input.outerHTML;
}

function makeTextInput(
  id,
  minlength = null,
  maxlength = null,
  required = false,
  pattern = null,
  placeholer = null
) {
  const input = makeInput(id, "text", required);
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

function makeInput(id, type, required = false) {
  const input = document.createElement("input");
  input.setAttribute("id", id);
  input.setAttribute("type", type);
  if (required) {
    input.setAttribute("required", "");
  }
  input.className = "input";
  return input;
}

export default columns;
