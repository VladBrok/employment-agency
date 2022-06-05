import fetchJson, { fetchBlob, fetchAllJson } from "./api.js";
import {
  makeFileInput,
  makeDateInput,
  makeNumberInput,
  makeTextInput,
  makeEmailInput,
  PATTERNS,
} from "./input.js";

class Column {
  constructor(
    realName,
    displayName,
    convertToInput,
    convertValue,
    convertFromValue
  ) {
    this.realName = realName;
    this.displayName = displayName;
    this.convertToInput = convertToInput;
    this.convertValue = convertValue ?? ((v) => v);
    this.convertFromValue = convertFromValue ?? ((v) => v);
  }

  async makeSelect(id, targetEndpoint, value, calledEndpoint) {
    const options = await fetchAllJson(targetEndpoint);

    if (calledEndpoint === targetEndpoint) {
      const disallowedWords = options
        .map((option) =>
          option[this.realName]
            .split("")
            .map(
              (letter) =>
                `${
                  letter.match(/\s/)
                    ? "[\\s]+"
                    : `[${letter.toLowerCase()}${letter.toUpperCase()}]`
                }`
            )
            .join("")
        )
        .join("|");

      return makeTextInput(
        id,
        value,
        1,
        20,
        true,
        `^(?!(?:${disallowedWords})(?=\/|$))${PATTERNS.LETTER_ONLY}`,
        "Значение должно быть уникальным и состоять из букв русского или латинского алфавита"
      );
    }

    if (!value) {
      value = options[0][this.realName];
    }
    return `<select class="select input" id=${id}>${options
      .map(
        (option) =>
          `<option data-id="${option.id}" value="${option.id}" ${
            option[this.realName].toLowerCase() === value.trim().toLowerCase()
              ? "selected"
              : ""
          }>${option[this.realName]}</option>`
      )
      .join("")}</select>`;
  }

  makeRadio(value, ...options) {
    const name = `${this.realName}-name`;

    return `<div>${options
      .map(
        (option, i) => `
          <div class="radio-wrapper">
            <label for="${option}${i}">${option}</label>
            <input type="radio" id="${option}${i}" name="${name}" class="radio-input" ${
          !value || option.toLowerCase() === value.trim().toLowerCase()
            ? "checked"
            : ""
        } value="${this.convertFromValue(option)}" required>
          </div>`
      )
      .join("")}</div>`;
  }

  makeLink(value, endpoint) {
    return `<a class="link-to-table input" data-endpoint="${endpoint}" value=${
      this.ids[value] ?? ""
    }>${value ?? "Выбрать... <input class='link-input input' required>"}</a>`;
  }

  async fetchName(id, endpoint, column) {
    const name = (await fetchJson({ endpoint: `${endpoint}/${id}` }))[column];
    if (this.ids) {
      this.ids[name] = id;
    } else {
      this.ids = { [name]: id };
    }
    return name;
  }
}

let districtSelectId = null;
let streetSelectId = null;
let firstStreetChangeCompleted = true;
let preventStreetsChange = false;

const columnInfo = [
  new Column("id", "№"),
  new Column(
    "employer_id",
    "Работодатель",
    function (_, value) {
      return this.makeLink(value, "/employers");
    },
    async function (id) {
      return await this.fetchName(id, "/employers", "employer");
    }
  ),
  new Column(
    "seeker_id",
    "Соискатель",
    function (_, value) {
      return this.makeLink(value, "/seekers");
    },
    async function (id) {
      return await this.fetchName(id, "/seekers", "first_name");
    }
  ),
  new Column("table_name", "название таблицы"),
  new Column("operation", "операция"),
  new Column("time_modified", "время совершения"),
  new Column("record_id", "id записи"),
  new Column("user_modified", "пользователь, совершивший операцию"),
  new Column("property", "тип собственности", async function (
    id,
    value,
    calledEndpoint
  ) {
    return await this.makeSelect(id, "/properties", value, calledEndpoint);
  }),
  new Column("position", "должность", async function (
    id,
    value,
    calledEndpoint
  ) {
    return await this.makeSelect(id, "/positions", value, calledEndpoint);
  }),
  new Column("status", "социальный статус", async function (
    id,
    value,
    calledEndpoint
  ) {
    return await this.makeSelect(id, "/statuses", value, calledEndpoint);
  }),
  new Column("type", "тип занятости", async function (
    id,
    value,
    calledEndpoint
  ) {
    return await this.makeSelect(
      id,
      "/employment_types",
      value,
      calledEndpoint
    );
  }),
  new Column("district", "район", async function (id, value, calledEndpoint) {
    districtSelectId = id;
    return await this.makeSelect(id, "/districts", value, calledEndpoint);
  }),
  new Column("street", "улица", async function (id, value, calledEndpoint) {
    streetSelectId = id;
    firstStreetChangeCompleted = false;
    preventStreetsChange = "/streets" === calledEndpoint;
    return await this.makeSelect(id, "/streets", value, calledEndpoint);
  }),
  new Column("postal_code", "почтовый индекс", (id, value, calledEndpoint) =>
    calledEndpoint === "/streets" ? makeNumberInput(id, value, 1, 100000) : null
  ),
  new Column("building_number", "номер дома", (id, value) =>
    makeNumberInput(id, value, 1, 1000000, true)
  ),
  new Column("employer", "работодатель", (id, value) =>
    makeTextInput(
      id,
      value,
      1,
      30,
      true,
      PATTERNS.LETTER_ONLY,
      "Название компании должно содержать только буквы русского или латинского алфавита"
    )
  ),
  new Column("phone", "номер телефона", (id, value) =>
    makeTextInput(
      id,
      value,
      null,
      null,
      true,
      "071[0-9]{7}",
      "Номер должен содержать 10 цифр и начинаться с 071"
    )
  ),
  new Column("email", "почта", makeEmailInput),
  new Column("employer_day", "дата размещения", () => null),
  new Column("salary_new", "предлагаемая зарплата", (id, value) =>
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
    (v) => (v === "True" ? "да" : "нет"),
    (v) => (v === "Да" ? "True" : "False")
  ),
  new Column("last_name", "фамилия", (id, value) =>
    makeTextInput(
      id,
      value,
      3,
      20,
      true,
      PATTERNS.LETTER_ONLY,
      "Фамилия должна содержать только буквы русского или латинского алфавита"
    )
  ),
  new Column("first_name", "имя", (id, value) =>
    makeTextInput(
      id,
      value,
      3,
      20,
      true,
      PATTERNS.LETTER_ONLY,
      "Имя должно содержать только буквы русского или латинского алфавита"
    )
  ),
  new Column("patronymic", "отчество", (id, value) =>
    makeTextInput(
      id,
      value,
      3,
      20,
      false,
      PATTERNS.LETTER_ONLY,
      "Отчество должно содержать только буквы русского или латинского алфавита"
    )
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
    (v) => (v === "True" ? "да" : "нет"),
    (v) => (v === "Да" ? "True" : "False")
  ),
  new Column(
    "pol",
    "пол",
    function (_, value) {
      return this.makeRadio(value, "Мужской", "Женский");
    },
    (v) => (v === "True" ? "мужской" : "женский"),
    (v) => (v === "Мужской" ? "True" : "False")
  ),
  new Column("education", "образование", (id, value) =>
    makeTextInput(id, value, 3, 20)
  ),
  new Column("seeker_day", "дата публикации", () => null),
  new Column("information", "информация", makeTextInput),
  new Column(
    "photo",
    "фото",
    function (id, path) {
      return `<div class="photo-container">${
        this.images[path]?.outerHTML ?? "<img class='photo'>"
      }${makeFileInput(id, path, "image/png, image/jpeg")}</div>`;
    },
    async function (path) {
      const img = document.createElement("img");
      img.classList.add("photo");
      if (path) {
        const photo = await fetchBlob(`/photos/${path}`);
        img.src = URL.createObjectURL(photo);
        if (this.images) {
          this.images[path] = img;
        } else {
          this.images = { [path]: img };
        }
      }

      return `<div class="photo-container">${path}${img.outerHTML}${
        path ? "" : "Фото нет"
      }</div>`;
    }
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

async function changeStreets(districtSelect) {
  if (preventStreetsChange) {
    return;
  }

  const districtId =
    districtSelect.options[districtSelect.selectedIndex].dataset.id;
  document.getElementById(streetSelectId).outerHTML = await columns[
    "street"
  ].makeSelect(streetSelectId, `/streets/district_id/${districtId}`);
}

document.body.addEventListener("change", async (e) => {
  if (e.target.id !== districtSelectId) {
    return;
  }

  await changeStreets(e.target);
});

const observer = new MutationObserver(async () => {
  if (firstStreetChangeCompleted || !document.getElementById(streetSelectId)) {
    return;
  }

  firstStreetChangeCompleted = true;
  await changeStreets(document.getElementById(districtSelectId));
});
observer.observe(document.body, { childList: true, subtree: true });

let columns = columnInfo.reduce((map, info) => {
  map[info.realName] = map[info.displayName] = info;
  return map;
}, {});
columns = new Proxy(columns, {
  get(target, prop) {
    if (prop in target) {
      return target[prop];
    }
    return new Column(prop, prop);
  },
});

export default columns;
