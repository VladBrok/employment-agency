import columns from "./columns.js";
import { makeTable, updateTable } from "./table.js";

const navigation = document.querySelector(".navigation");
const main = document.querySelector(".main");

navigation.addEventListener("click", async (e) => {
  const endpoint = e.target.dataset.endpoint;

  if (endpoint) {
    const title = e.target.textContent;
    main.innerHTML = await makeTable(title, endpoint);
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
    makeForm(e.target, "Создать");
    return;
  }

  if (e.target.tagName === "TD" && e.target.closest(".body") !== null) {
    const values = Array.from(e.target.parentElement.querySelectorAll("td"))
      .map((x) => x.textContent)
      .slice(1);

    console.log(values);

    makeForm(e.target, "Обновить", values);
    return;
  }
});

async function makeForm(tableChild, buttonText, values = null) {
  const calledEndpoint = tableChild.closest("[data-endpoint]").dataset.endpoint;
  const names = Array.from(
    tableChild.closest("[data-endpoint]").querySelectorAll("th")
  )
    .slice(1) // skip an id
    .map((x) => x.textContent);

  const form = `
    <form class="crud-form">
      ${(
        await Promise.all(
          names.map(async (name, i) => {
            const input = await columns[name]?.convertToInput(
              `name${i}`,
              values?.[i],
              calledEndpoint
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
      <button type="submit" class="search-button button">${buttonText}</button>
    </form>`;
  main.innerHTML = form;
}
