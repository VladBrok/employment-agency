import columns from "./columns.js";
import endpoints from "./endpoints.js";
import { makeTable } from "./table.js";

async function makeForm(
  tableChild,
  buttonText,
  className,
  values = null,
  entityId = null
) {
  let endpoint = tableChild.closest("[data-endpoint]").dataset.endpoint;
  const subEndpoint = endpoint.indexOf("/", 1);
  if (subEndpoint !== -1) {
    endpoint = endpoint.slice(0, subEndpoint);
  }

  const names = Array.from(
    tableChild.closest("[data-endpoint]").querySelectorAll("th")
  )
    .slice(1) // skip an id
    .map((x) => x.textContent);

  const form = `
    <form class="crud-form" data-endpoint="${endpoint}" data-id="${entityId}">
      ${(
        await Promise.all(
          names.map(async (name, i) => {
            const input = await columns[name]?.convertToInput(
              `name${i}`,
              values?.[i],
              endpoint
            );
            return input === null
              ? ""
              : `<div class="element">
                  <label for="name${i}">${name}:</label>
                  ${input}
                </div>`;
          })
        )
      ).join("")}
      <button type="submit" class="search-button button ${className}">${buttonText}</button>
    </form>`;

  const id = tableChild.closest("tr")?.querySelector("td").textContent; // todo: make data-id
  const childTables =
    entityId == null
      ? []
      : await Promise.all(
          endpoints[endpoint]?.children.map(
            async (endpoint) =>
              await makeTable(endpoints[endpoint].title, endpoint, entityId)
          ) ?? []
        );

  return `${form}${childTables.join("")}`;
}

async function sendForm(callback, e) {
  const form = document.querySelector(".crud-form");
  const names = [];
  const values = [];

  for (const input of form.querySelectorAll(".input, input:checked")) {
    if (input.checkValidity && !input.checkValidity()) {
      return;
    }

    values.push(
      input.getAttribute("type") === "file"
        ? input.files[0] ?? ""
        : input.value ?? input.getAttribute("value")
    );
  }

  for (const label of form.querySelectorAll("label")) {
    if (!label.textContent.endsWith(":")) {
      continue;
    }

    let name = columns[label.textContent.slice(0, -1)].realName;
    if (
      document.getElementById(label.getAttribute("for"))?.tagName === "SELECT"
    ) {
      name += "_id";
    }
    if (name === "position_id" && form.dataset.endpoint === "/seekers") {
      name = "speciality_id";
    }
    if (name === "type_id" && form.dataset.endpoint === "/applications") {
      name = "employment_type_id";
    }

    names.push(name);
  }

  const formData = new FormData();
  names.map((name, i) => formData.append(name, values[i]));
  callback(form, formData);
  e.preventDefault();
}

export { makeForm, sendForm };
