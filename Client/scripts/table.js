import endpoints from "./endpoints.js";
import columns from "./columns.js";
import fetchJson from "./api.js";

async function makeTable(title, endpoint, id = null) {
  const endpointForFetching = id ? `${endpoint}/${id}` : endpoint;
  const parameters = endpoints[endpoint].parameters;
  const data = await fetchJson({
    endpoint: endpointForFetching,
    parameterValues: parameters.map((p) => p.defaultValue),
  });

  return `
  <div class="table-container" data-endpoint="${endpointForFetching}" data-access="${
    endpoints[endpoint].access
  }">
    <h2 class="title">${title}</h2>  
    <div class="before-table">
      <table>
        <tr class="search">
          <td><label for="search">Поиск по подстроке:</label></td>
          <td><input type="text" id="search" class="input" required></td>
          <td><button class="search-button button find">Поиск</button></td>
        </tr>
        ${
          parameters
            .map(
              (param, i) => `
                <tr class="search">
                  <td><label for="search${i}">${param.name}:</label></td>
                  <td>${param.convertToInput(`search${i}`)}</td>
                  <td><button class="search-button button enter-button">Ввод</button></td>
                </tr>`
            )
            .join("") ?? ""
        }
      </table>
      <img class='create' src='images/create.png'></img>
    </div>
    <div class="table-wrapper">
    <table class="table">
      <thead class="head">
        ${extractColumns(data)}
      </thead>
      <tbody class="body">
        ${await extractRows(data)}
      </tbody>
    </table>
    </div>
    <div class="pages">
      <div class="previous-page element button disabled">❮</div>
      <div class="current-page element button disabled">1</div>
      <div class="next-page element button">❯</div>
    </div>
  </div>`;
}

function extractColumns(data) {
  return data?.length
    ? `<tr><th>${Object.keys(data[0])
        .map((k) => columns[k]?.displayName ?? k)
        .join("</th><th>")}</th></tr>`
    : "";
}

async function extractRows(data) {
  return data?.length
    ? `<tr>${(
        await Promise.all(
          data.map(
            async (d) =>
              `<td>${(
                await Promise.all(
                  Object.entries(d).map(
                    async ([colName, value]) =>
                      await (columns[colName]?.convertValue(value) ?? value)
                  )
                )
              ).join("</td><td>")}</td>`
          )
        )
      ).join("</tr><tr>")}</tr>`
    : "<tr class='not-a-data-row'><td colspan='100'><h2 class='title'>Результатов нет.</h2></td></tr>";
}

async function updateTable(tableChild, page = 0) {
  const tableContainer = tableChild.closest("[data-endpoint]");
  const endpoint = tableContainer.dataset.endpoint;

  const inputs = Array.from(tableContainer.querySelectorAll(".input"));
  const filter = inputs[0].value;
  const parameterValues = inputs.slice(1).map((x) => x.value);

  const data = await fetchJson({
    endpoint,
    page,
    filter,
    parameterValues,
  });
  tableContainer.querySelector(".body").innerHTML = await extractRows(data);
}

export { makeTable, updateTable };
