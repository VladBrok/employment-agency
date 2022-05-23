const URL = "https://localhost:7288/api";
const PAGE_SIZE = 15;

const navigation = document.querySelector(".navigation");
const main = document.querySelector(".main");

navigation.addEventListener("click", async (e) => {
  const endpoint = e.target.dataset.endpoint;

  if (endpoint) {
    const title = e.target.textContent;
    const response = await fetch(
      `${URL}${endpoint}?page=0&pageSize=${PAGE_SIZE}`
    );
    const data = await response.json();
    makeTable(title, data, endpoint);
  }
});

main.addEventListener("click", async (e) => {
  if (e.target.classList.contains("previous-page")) {
    const page = e.target.parentElement.querySelector(".current-page");
    page.textContent--;
    const endpoint = e.target.closest("[data-endpoint]").dataset.endpoint;

    const response = await fetch(
      `${URL}${endpoint}?page=${+page.textContent - 1}&pageSize=${PAGE_SIZE}`
    );
    const data = await response.json();

    e.target.parentElement.parentElement.querySelector(
      ".body"
    ).innerHTML = `<tr>${data
      .map((d) => `<td>${Object.values(d).join("</td><td>")}</td>`)
      .join("</tr><tr>")}</tr>`;

    if (+page.textContent === 1) {
      e.target.classList.add("disabled");
    }
    return;
  }

  if (e.target.classList.contains("next-page")) {
    const page = e.target.parentElement.querySelector(".current-page");
    const endpoint = e.target.closest("[data-endpoint]").dataset.endpoint;

    const response = await fetch(
      `${URL}${endpoint}?page=${+page.textContent}&pageSize=${PAGE_SIZE}`
    );
    const data = await response.json();

    e.target.parentElement.parentElement.querySelector(
      ".body"
    ).innerHTML = `<tr>${data
      .map((d) => `<td>${Object.values(d).join("</td><td>")}</td>`)
      .join("</tr><tr>")}</tr>`;
    page.textContent++;

    if (+page.textContent === 2) {
      e.target.parentElement
        .querySelector(".previous-page")
        .classList.remove("disabled");
    }
  }
});

function makeTable(title, data, endpoint) {
  const columnHeaders = Object.keys(data[0]);

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
        <tr><th>${columnHeaders.join("</th><th>")}</th></tr>
      </thead>
      <tbody class="body">
        <tr>${data
          .map((d) => `<td>${Object.values(d).join("</td><td>")}</td>`)
          .join("</tr><tr>")}</tr>
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
