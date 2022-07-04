import endpoints from "./endpoints.js";
import columns from "./columns.js";
import fetchJson, { PAGE_SIZE } from "./api.js";
import loadingDecorator from "./loading.js";

document.addEventListener("click", loadingDecorator(handleClick));

async function handleClick(e) {
  if (e.target.classList.contains("find")) {
    await updateTable(e.target);
    return;
  }

  if (e.target.classList.contains("change-page")) {
    const page = document.querySelector(".current-page");
    e.target.classList.contains("previous-page")
      ? page.textContent--
      : page.textContent++;

    await updateTable(page, page.textContent - 1);
  }
}

async function makeTable({
  endpoint,
  id = null,
  title = endpoints[endpoint].title,
  chartType = "none",
}) {
  const endpointForFetching = id ? `${endpoint}/${id}` : endpoint;
  const parameters = endpoints[endpoint].parameters;

  const data = await fetchJson({
    endpoint: endpointForFetching,
    parameterValues: parameters.map((p) => p.defaultValue),
    pageSize: PAGE_SIZE + 1,
  });
  const dataForPage = data?.length > PAGE_SIZE ? data.slice(0, -1) : data;

  return `
  <div class="table-container" data-endpoint="${endpointForFetching}" data-access="${
    endpoints[endpoint].access
  }">
    <h2 class="title">${title}</h2>
    ${
      data?.length
        ? `
    <div class="before-table">
      <form onSubmit="return false;" class="table-form">
        <div class="search">
          <span class='td'><label>Столбец для поиска:</label></span>
          <span class='td'>
            <select class="select input">
              ${extractColumnNames(dataForPage)
                .filter((name) => columns[name].isFilterable)
                .map((name) => `<option>${name}</option>`)
                .join("")}
            </select>
          </span>
        </div>
        <div class="search">
          <span class='td'><label for="search">Подстрока для поиска:</label></span>
          <span class='td'><input type="text" id="search" class="input"></span>
          <span class='td'><button class="search-button button find">Поиск</button></span>
        </div>
        ${parameters
          .map(
            (param, i) => `
                <div class='search'>
                  <span class='td'><label for="search${i}">${
              param.name
            }:</label></span>
                  <span class='td'>${param.convertToInput(`search${i}`)}</span>
                  <span class='td'><button class="search-button button find">Ввод</button></span>
                </div>`
          )
          .join("")}
      </form>
      <div class="actions">
      <img class='chart' src='images/chart.png' alt="Нарисовать диаграмму" title="Нарисовать диаграмму" data-chart="${chartType}">
      <select class="download" title="Скачать отчет">
        <option selected disabled></option>
        <option>html</option>
        <option>excel</option>
      </select>
      <img class='create' src='images/create.png' alt="Создать" title="Создать новую запись">
      <img src="images/delete.png" alt="Удалить" title="Удалить" class="delete delete-many">
      </div>
    </div>`
        : ""
    }
    <div class="table-wrapper">
    <table class="table">
      <thead class="head">
        ${extractColumns(dataForPage)}
      </thead>
      <tbody class="body">
        ${await extractRows(dataForPage)}
      </tbody>
    </table>
    </div>
    <div class="pages">
      <div class="previous-page change-page element button disabled">❮</div>
      <div class="current-page element button disabled">1</div>
      <div class="next-page change-page element button ${
        data?.length > PAGE_SIZE ? "" : "disabled"
      }">❯</div>
    </div>
  </div>`;
}

function extractColumns(data) {
  return data?.length
    ? `<tr><th>
    ${extractColumnNames(data).join("</th><th>")}</th></tr>`
    : "";
}

function extractColumnNames(data) {
  return Object.keys(data[0]).map((key) => columns[key].displayName);
}

async function extractRows(data) {
  return data?.length
    ? `${(await Promise.all(data.map(extractCells))).join("")}`
    : "<tr class='not-a-data-row'><td colspan='100'><h2 class='title'>Результатов нет.</h2></td></tr>";
}

async function extractCells(row) {
  return `<tr><td>${(
    await Promise.all(
      Object.entries(row).map(
        async ([colName, value], _, entries) =>
          await columns[colName].convertValue(value, entries)
      )
    )
  ).join("</td><td>")}</td></tr>`;
}

async function updateTable(tableChild, page = 0) {
  document.querySelector(".current-page").textContent = page + 1;
  adjustButtonAvailability(".previous-page", page === 0);

  const data = await fetchJsonFromTable({
    tableChild,
    page,
    pageSize: PAGE_SIZE + 1,
  });
  const dataForPage = data?.length > PAGE_SIZE ? data.slice(0, -1) : data;

  adjustButtonAvailability(".next-page", data?.length <= PAGE_SIZE);
  tableChild.closest("[data-endpoint]").querySelector(".body").innerHTML =
    await extractRows(dataForPage);
}

async function fetchJsonFromTable({
  tableChild,
  page = 0,
  pageSize = PAGE_SIZE,
}) {
  const tableContainer = tableChild.closest("[data-endpoint]");
  const endpoint = tableContainer.dataset.endpoint;
  const inputs = Array.from(tableContainer.querySelectorAll("input.input"));
  const parameterValues = inputs.slice(1).map((x) => x.value);
  const filter = inputs[0].value.trim().toLowerCase();

  if (filter === "") {
    return await fetchJson({
      endpoint,
      page,
      filter,
      parameterValues,
      pageSize,
    });
  }

  const data = await fetchJson({ endpoint, parameterValues, pageSize: 1e6 });
  const filterColumn =
    columns[tableContainer.querySelector(".select").value].realName;
  const filterColumnIndex = Object.keys(data[0]).indexOf(
    Object.keys(data[0]).find((key) => filterColumn === key)
  );

  const filteredData = data.filter((d) => {
    const entries = Object.entries(d);
    const [name, value] = entries[filterColumnIndex];
    return columns[name]
      .convertValue(value, entries)
      .toLowerCase()
      .includes(filter);
  });
  return filteredData.slice(page * PAGE_SIZE, page * PAGE_SIZE + pageSize);
}

function adjustButtonAvailability(selector, shouldDisable) {
  const classList = document.querySelector(selector).classList;
  shouldDisable ? classList.add("disabled") : classList.remove("disabled");
}

export { makeTable, fetchJsonFromTable };
