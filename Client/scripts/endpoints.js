import { makeNumberInput } from "./inputs.js";

class Parameter {
  constructor(name, convertToInput, defaultValue) {
    this.name = name;
    this.convertToInput = convertToInput;
    this.defaultValue = defaultValue;
  }
}

class Endpoint {
  constructor(
    main,
    children = [],
    access = "readonly",
    parameters = [],
    title = "?"
  ) {
    this.main = main;
    this.children = children;
    this.access = access;
    this.parameters = parameters;
    this.title = title;
  }
}

const endpointInfo = [
  new Endpoint("/vacancies/employer_id", [], "full", [], "Вакансии"),
  new Endpoint(
    "/applications/seeker_id",
    [],
    "full",
    [],
    "Заявки на трудоустройство"
  ),
  new Endpoint("/employers/address_id", [], "full", [], "Работодатели"),
  new Endpoint("/seekers/address_id", [], "full", [], "Соискатели"),
  new Endpoint(
    "/applications/position_id",
    [],
    "full",
    [],
    "Заявки на трудоустройство"
  ),
  new Endpoint("/seekers/position_id", [], "full", [], "Соискатели"),
  new Endpoint("/vacancies/position_id", [], "full", [], "Вакансии"),
  new Endpoint("/employers/property_id", [], "full", [], "Работодатели"),
  new Endpoint("/streets/district_id", [], "full", [], "Улицы"),
  new Endpoint(
    "/applications/employment_type_id",
    [],
    "full",
    [],
    "Заявки на трудоустройство"
  ),
  new Endpoint("/seekers/status_id", [], "full", [], "Соискатели"),
  new Endpoint("/employers", ["/vacancies/employer_id"], "full"),
  new Endpoint("/seekers", ["/applications/seeker_id"], "full"),
  new Endpoint("/vacancies", [], "full"),
  new Endpoint("/applications", [], "full"),
  new Endpoint(
    "/addresses",
    ["/employers/address_id", "/seekers/address_id"],
    "full"
  ),
  new Endpoint(
    "/positions",
    [
      "/applications/position_id",
      "/seekers/position_id",
      "/vacancies/position_id",
    ],
    "full"
  ),
  new Endpoint("/properties", ["/employers/property_id"], "full"),
  new Endpoint("/streets", [], "full"),
  new Endpoint("/districts", ["/streets/district_id"], "full"),
  new Endpoint(
    "/employment_types",
    ["/applications/employment_type_id"],
    "full"
  ),
  new Endpoint("/statuses", ["/seekers/status_id"], "full"),
  new Endpoint("/special/average_seeker_ages_by_positions"),
  new Endpoint("/special/vacancies_and_salaries"),
  new Endpoint("/special/employer_addresses"),
  new Endpoint("/special/employment_types_and_salaries"),
  new Endpoint("/special/employers_and_vacancies"),
  new Endpoint("/special/seekers_and_applications"),
  new Endpoint("/special/num_vacancies_from_each_employer"),
  new Endpoint("/special/applications_without_experience"),
  new Endpoint("/special/applications_percent_after", [], "readonly", [
    new Parameter(
      "Год",
      (id) => makeNumberInput(id, "2017", 1980, 2022),
      "2017"
    ),
  ]),
  new Endpoint("/special/applications_percent_by_positions_after"),
  new Endpoint("/special/application_count_by_positions", [], "readonly", [
    new Parameter(
      "Год",
      (id) => makeNumberInput(id, "2017", 1980, 2022),
      "2017"
    ),
    new Parameter("Месяц", (id) => makeNumberInput(id, "5", 1, 12), "5"),
  ]),
  new Endpoint("/special/num_applications_for_each_employment_type"),
  new Endpoint("/special/seekers_in_district"),
  new Endpoint("/special/seekers_born_after"),
  new Endpoint("/special/seekers_whose_total_experience_exceeds"),
  new Endpoint("/special/employers_with_property"),
  new Endpoint("/special/vacancies_posted_on"),
  new Endpoint("/special/latest_vacancy_of_employers_whose_name_contains"),
  new Endpoint(
    "/special/positions_from_open_vacancies_whose_average_salary_exceeds"
  ),
  new Endpoint("/special/max_salaries_for_position"),
];

const endpoints = {};
for (const info of endpointInfo) {
  endpoints[info.main] = info;
}

export default endpoints;
