import endpoints from "./endpoints.js";
import columns from "./columns.js";
import fetchJson from "./api.js";

async function makeTable(title, endpoint) {
  const parameters = endpoints[endpoint].parameters;
  const data = await fetchJson({
    endpoint,
    parameters: parameters.map((p) => p.defaultValue),
  });

  return `
  <div class="table-container" data-endpoint="${endpoint}" data-access="${
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
              (param, i) =>
                `<tr class="search">
        <td><label for="search${i}">${param.name}:</label></td>
        <td><input type="${param.type}" id="search${i}" class="input" value="${param.defaultValue}"></td>
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
        ${extractRows(data)}
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
        .map((k) => columns[k]?.displayName ?? k) // todo: make proxy to return k
        .join("</th><th>")}</th></tr>`
    : "";
}

function extractRows(data) {
  return data?.length
    ? `<tr>${data
        .map(
          (d) =>
            `<td>${Object.entries(d)
              .map(
                ([colName, value]) =>
                  columns[colName]?.convertValue(value) ?? value // todo: make proxy to return value
              )
              .join("</td><td>")}</td>`
        )
        .join("</tr><tr>")}</tr>`
    : "<tr class='not-a-data-row'><td colspan='100'><h2 class='title'>Результатов нет.</h2></td></tr>";
}

async function updateTable(tableChild, page = 0) {
  const tableContainer = tableChild.closest("[data-endpoint]");
  const endpoint = tableContainer.dataset.endpoint;

  const inputs = Array.from(tableContainer.querySelectorAll(".input"));
  const filter = inputs[0].value;
  const parameters = inputs.slice(1).map((x) => x.value);

  const data = await fetchJson({ endpoint, page, filter, parameters });
  tableContainer.querySelector(".body").innerHTML = extractRows(data);
}

export { makeTable, updateTable };
