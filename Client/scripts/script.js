const URL = "https://localhost:7288/api";
const PAGE_SIZE = 15;

const navigation = document.querySelector(".navigation");
const main = document.querySelector(".main");

navigation.addEventListener("click", async (e) => {
  const endpoint = e.target.dataset.endpoint;

  if (endpoint) {
    const title = e.target.textContent;
    const data = await fetchJson(endpoint, 0);
    makeTable(title, data, endpoint);
  }
});

main.addEventListener("click", async (e) => {
  if (e.target.classList.contains("previous-page")) {
    const page = e.target.parentElement.querySelector(".current-page");

    await changePage(page, -1);

    if (+page.textContent === 1) {
      e.target.classList.add("disabled");
    }
    return;
  }

  if (e.target.classList.contains("next-page")) {
    const page = e.target.parentElement.querySelector(".current-page");

    await changePage(page, +1);

    if (+page.textContent === 2) {
      e.target.parentElement
        .querySelector(".previous-page")
        .classList.remove("disabled");
    }
  }
});

async function fetchJson(endpoint, page) {
  const response = await fetch(
    `${URL}${endpoint}?page=${+page}&pageSize=${PAGE_SIZE}`
  );
  return await response.json();
}

function makeTable(title, data, endpoint) {
  main.insertAdjacentHTML(
    "beforeend",
    `
  <div class="table-container" data-endpoint="${endpoint}">
    <h2 class="title">${title}</h2>  
    <div class="before-table">
      <div class="search">
        <label for="search">Поиск по подстроке:</label>
        <input type="text" id="search" name="search" class="input">
        <button class="search-button button">Поиск</button>
      </div>
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
  </div>`
  );
}

function extractColumns(data) {
  return `<tr><th>${Object.keys(data[0]).join("</th><th>")}</th></tr>`;
}

function extractRows(data) {
  return `<tr>${data
    .map((d) => `<td>${Object.values(d).join("</td><td>")}</td>`)
    .join("</tr><tr>")}</tr>`;
}

async function changePage(page, increment) {
  page.textContent = +page.textContent + increment;

  const tableContainer = page.closest("[data-endpoint]");
  const endpoint = tableContainer.dataset.endpoint;
  const data = await fetchJson(endpoint, page.textContent - 1);

  tableContainer.querySelector(".body").innerHTML = extractRows(data);
}
