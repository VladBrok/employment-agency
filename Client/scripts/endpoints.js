class Parameter {
  constructor(name, type, defaultValue) {
    this.name = name;
    this.type = type;
    this.defaultValue = defaultValue;
  }
}

class Endpoint {
  constructor(main, children = [], access = "readonly", parameters = []) {
    this.main = main;
    this.children = children;
    this.access = access;
    this.parameters = parameters;
  }
}

const endpointInfo = [
  new Endpoint("/employers", ["/vacancies"], "full"),
  new Endpoint("/seekers", ["/applications"], "full"),
  new Endpoint("/vacancies", [], "full"),
  new Endpoint("/applications", [], "full"),
  new Endpoint("/addresses", ["/employers", "/seekers"], "full"),
  new Endpoint(
    "/positions",
    ["/applications", "/seekers", "/vacancies"],
    "full"
  ),
  new Endpoint("/properties", ["/employers"], "full"),
  new Endpoint("/streets", [], "full"),
  new Endpoint("/districts", ["/streets"], "full"),
  new Endpoint("/employment_types", ["/applications"], "full"),
  new Endpoint("/statuses", ["/seekers"], "full"),
  new Endpoint("/special/average_seeker_ages_by_positions"),
  new Endpoint("/special/vacancies_and_salaries"),
  new Endpoint("/special/employer_addresses"),
  new Endpoint("/special/employment_types_and_salaries"),
  new Endpoint("/special/employers_and_vacancies"),
  new Endpoint("/special/seekers_and_applications"),
  new Endpoint("/special/num_vacancies_from_each_employer"),
  new Endpoint("/special/applications_without_experience"),
  new Endpoint("/special/applications_percent_after", [], "readonly", [
    new Parameter("Год", "number", 2017),
  ]),
  new Endpoint("/special/applications_percent_by_positions_after"),
  new Endpoint("/special/application_count_by_positions", [], "readonly", [
    new Parameter("Год", "number", 2017),
    new Parameter("Месяц", "number", 5),
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
