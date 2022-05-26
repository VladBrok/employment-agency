import columns from "./columns.js";
import values from "./values.js";
import fetchJson from "./api.js";

const navigation = document.querySelector(".navigation");
const main = document.querySelector(".main");

navigation.addEventListener("click", async (e) => {
  const endpoint = e.target.dataset.endpoint;

  if (endpoint) {
    const title = e.target.textContent;
    const parameterNames = e.target.dataset.parameters?.split(",");
    const data = await fetchJson({
      endpoint,
      parameters: parameterNames?.map((p) => values[p].defaultValue),
    });
    makeTable(title, data, endpoint, parameterNames);
  }
});

main.addEventListener("click", async (e) => {
  if (e.target.classList.contains("previous-page")) {
    const page = e.target.parentElement.querySelector(".current-page");
    page.textContent--;

    await updateTable(page, page.textContent - 1);

    if (+page.textContent === 1) {
      e.target.classList.add("disabled");
    }
    return;
  }

  if (e.target.classList.contains("next-page")) {
    const page = e.target.parentElement.querySelector(".current-page");
    page.textContent++;

    await updateTable(page, page.textContent - 1);

    if (+page.textContent === 2) {
      e.target.parentElement
        .querySelector(".previous-page")
        .classList.remove("disabled");
    }
    return;
  }

  if (e.target.classList.contains("search-button")) {
    await updateTable(e.target);
    return;
  }

  if (e.target.classList.contains("create")) {
    // dup with tr
    const labels = Array.from(
      e.target.closest("[data-endpoint]").querySelectorAll("th")
    )
      .map((x) => x.textContent)
      .slice(1); // skip an id
    const form = `
      <form class="crud-form">
        ${(
          await Promise.all(
            labels.map(
              async (l, i) => `
                <div class="element">
                  <label for="l${i}">${l}:</label>
                  ${await columns[l]?.convertToInput(`l${i}`)}
                </div>`
            )
          )
        ).join("")}
        <button type="submit" class="search-button button">Создать</button>
      </form>`;
    main.innerHTML = form;
    return;
  }

  if (e.target.tagName === "TD" && e.target.closest(".body") !== null) {
    // dup with create
    const labels = Array.from(
      e.target.closest("[data-endpoint]").querySelectorAll("th")
    )
      .map((x) => x.textContent)
      .slice(1); // skip an id
    const values = Array.from(e.target.parentElement.querySelectorAll("td"))
      .map((x) => x.textContent)
      .slice(1);

    const form = `
      <form class="crud-form">
        ${(
          await Promise.all(
            labels.map(
              async (l, i) => `
                <div class="element">
                  <label for="l${i}">${l}:</label>
                  ${await columns[l]?.convertToInput(`label${i}`, values[i])}
                </div>`
            )
          )
        ).join("")}
        <button type="submit" class="search-button button">Обновить</button>
      </form>`;
    main.innerHTML = form;
    return;
  }
});

function makeTable(title, data, endpoint, parameterNames) {
  main.innerHTML = `
  <div class="table-container" data-endpoint="${endpoint}">
    <h2 class="title">${title}</h2>  
    <div class="before-table">
      <table>
        <tr class="search">
          <td><label for="search">Поиск по подстроке:</label></td>
          <td><input type="text" id="search" class="input" required></td>
          <td><button class="search-button button find">Поиск</button></td>
        </tr>
        ${
          parameterNames
            ?.map(
              (p, i) =>
                `<tr class="search">
        <td><label for="search${i}">${p}:</label></td>
        <td><input type="${values[p].type}" id="search${i}" class="input" value="${values[p].defaultValue}"></td>
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
              .join("</td><td>")}
            </td>`
        )
        .join("</tr><tr>")}</tr>`
    : "<tr><td colspan='100'><h2 class='title'>Результатов нет.</h2></td></tr>";
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
