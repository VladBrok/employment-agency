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

export default makeForm;
