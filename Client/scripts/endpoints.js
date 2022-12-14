import { makeDateInput, makeNumberInput, makeTextInput } from "./input.js";

class Parameter {
  constructor(name, convertToInput) {
    this.name = name;
    this.convertToInput = convertToInput;
    this.defaultValue = [...convertToInput("").matchAll(/value="(.+?)"/g)].map(
      (m) => m[1]
    )[0];
  }
}

class Endpoint {
  constructor({
    main,
    children = [],
    parameters = [],
    title = "?",
    access = null,
  }) {
    this.main = main;
    this.children = children;
    this.parameters = parameters;
    this.title = title;
    this.access =
      access ?? (main.indexOf("special") === -1 ? "full" : "readonly");
  }
}

const endpointInfo = [
  new Endpoint({ main: "/vacancies/employer_id", title: "Вакансии" }),
  new Endpoint({
    main: "/applications/seeker_id",
    title: "Заявки на трудоустройство",
  }),
  new Endpoint({ main: "/employers/district_id", title: "Работодатели" }),
  new Endpoint({ main: "/seekers/district_id", title: "Соискатели" }),
  new Endpoint({
    main: "/applications/position_id",
    title: "Заявки на трудоустройство",
  }),
  new Endpoint({ main: "/seekers/education_id", title: "Образование" }),
  new Endpoint({
    main: "/seekers/registration_city_id",
    title: "Соискатели",
  }),
  new Endpoint({ main: "/vacancies/position_id", title: "Вакансии" }),
  new Endpoint({ main: "/employers/property_id", title: "Работодатели" }),
  new Endpoint({ main: "/districts/city_id", title: "Районы" }),
  new Endpoint({
    main: "/applications/employment_type_id",
    title: "Заявки на трудоустройство",
  }),
  new Endpoint({ main: "/seekers/status_id", title: "Соискатели" }),
  new Endpoint({
    main: "/employers",
    children: ["/vacancies/employer_id"],
    title: "Работодатели",
  }),
  new Endpoint({
    main: "/seekers",
    children: ["/applications/seeker_id"],
    title: "Соискатели",
  }),
  new Endpoint({ main: "/vacancies" }),
  new Endpoint({ main: "/applications" }),
  new Endpoint({
    main: "/positions",
    children: ["/applications/position_id", "/vacancies/position_id"],
  }),
  new Endpoint({ main: "/properties", children: ["/employers/property_id"] }),
  new Endpoint({
    main: "/districts",
    children: ["/employers/district_id", "/seekers/district_id"],
  }),
  new Endpoint({
    main: "/cities",
    children: ["/districts/city_id", "/seekers/registration_city_id"],
  }),
  new Endpoint({
    main: "/educations",
    children: ["/seekers/education_id"],
  }),
  new Endpoint({
    main: "/employment_types",
    children: ["/applications/employment_type_id"],
  }),
  new Endpoint({ main: "/statuses", children: ["/seekers/status_id"] }),
  new Endpoint({ main: "/special/average_seeker_ages_by_positions" }),
  new Endpoint({ main: "/special/vacancies_and_salaries" }),
  new Endpoint({ main: "/special/employer_addresses" }),
  new Endpoint({ main: "/special/employment_types_and_salaries" }),
  new Endpoint({ main: "/special/employers_and_vacancies" }),
  new Endpoint({ main: "/special/seekers_and_applications" }),
  new Endpoint({ main: "/special/num_vacancies_from_each_employer" }),
  new Endpoint({ main: "/special/total_vacancies_including_not_ended" }),
  new Endpoint({ main: "/special/num_of_seekers_with_university_education" }),
  new Endpoint({ main: "/special/employers_all_and_by_districts" }),
  new Endpoint({ main: "/special/seekers_with_even_average_experience" }),
  new Endpoint({
    main: "/special/application_count_of_seekers_whose_name_starts_with",
    parameters: [new Parameter("Символы", (id) => makeTextInput(id, "ал"))],
  }),
  new Endpoint({
    main: "/special/min_salary_of_employer_with_name",
    parameters: [new Parameter("Компания", (id) => makeTextInput(id, "abaa"))],
  }),
  new Endpoint({
    main: "/special/applications_with_position",
    parameters: [
      new Parameter("Должность", (id) => makeTextInput(id, "Промоутер")),
    ],
  }),
  new Endpoint({
    main: "/special/seekers_not_registered_in",
    parameters: [
      new Parameter("Город", (id) => makeTextInput(id, "Амстердам")),
    ],
  }),
  new Endpoint({
    main: "/special/salaries_in_comparison_with",
    parameters: [
      new Parameter("Зарплата", (id) => makeNumberInput(id, "52000")),
    ],
  }),
  new Endpoint({
    main: "/special/applications_without_experience",
    parameters: [
      new Parameter("Должность", (id) => makeTextInput(id, "Промоутер")),
    ],
  }),
  new Endpoint({
    main: "/special/applications_percent_after",
    parameters: [new Parameter("Год", (id) => makeNumberInput(id, "2017"))],
  }),
  new Endpoint({
    main: "/special/applications_percent_by_positions_after",
    parameters: [new Parameter("Год", (id) => makeNumberInput(id, "2017"))],
  }),
  new Endpoint({
    main: "/special/application_count_by_positions",
    parameters: [
      new Parameter("Год", (id) => makeNumberInput(id, "2017")),
      new Parameter("Месяц", (id) => makeNumberInput(id, "5")),
    ],
  }),
  new Endpoint({ main: "/special/num_applications_for_each_employment_type" }),
  new Endpoint({
    main: "/special/seekers_in_district",
    parameters: [
      new Parameter("Район", (id) => makeTextInput(id, "Ворошиловский")),
    ],
  }),
  new Endpoint({
    main: "/special/seekers_born_after",
    parameters: [
      new Parameter("Дата", (id) => makeDateInput(id, "03.02.2002")),
    ],
  }),
  new Endpoint({
    main: "/special/seekers_whose_total_experience_exceeds",
    parameters: [new Parameter("Опыт", (id) => makeNumberInput(id, "5"))],
  }),
  new Endpoint({
    main: "/special/employers_with_property",
    parameters: [
      new Parameter("Тип собственности", (id) => makeTextInput(id, "Частная")),
    ],
  }),
  new Endpoint({
    main: "/special/vacancies_posted_on",
    parameters: [
      new Parameter("Дата", (id) => makeDateInput(id, "02.05.2019")),
    ],
  }),
  new Endpoint({
    main: "/special/latest_vacancy_of_employers_whose_name_contains",
    parameters: [
      new Parameter("Подстрока имени", (id) => makeTextInput(id, "AAA")),
    ],
  }),
  new Endpoint({
    main: "/special/positions_from_open_vacancies_whose_average_salary_exceeds",
    parameters: [
      new Parameter("Зарплата", (id) => makeNumberInput(id, "52000")),
    ],
  }),
  new Endpoint({
    main: "/special/max_salaries_for_position",
    parameters: [
      new Parameter("Должность", (id) => makeTextInput(id, "Промоутер")),
    ],
  }),
  new Endpoint({ main: "/change_log", access: "read-delete" }),
  new Endpoint({ main: "/generate" }),
];

const endpoints = endpointInfo.reduce((map, info) => {
  map[info.main] = info;
  return map;
}, {});

export default endpoints;
