import { makeTable, updateTable } from "./table.js";
import { makeForm, sendForm } from "./form.js";
import { post, put } from "./api.js";
import endpoints from "./endpoints.js";

const navigation = document.querySelector(".navigation");
const main = document.querySelector(".main");

navigation.addEventListener("click", handleNavigationClick);

let pageSnapshot = null;

main.addEventListener("click", async (e) => {
  if (e.target.classList.contains("link-to-table")) {
    const endpoint = e.target.dataset.endpoint;
    pageSnapshot = main.innerHTML;
    main.innerHTML = await makeTable(endpoints[endpoint].title, endpoint);
    return;
  }

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

  if (e.target.classList.contains("update-button")) {
    await sendForm(
      async (form, data) =>
        await put(form.dataset.endpoint, form.dataset.id, data),
      e
    );
    return;
  }

  if (e.target.classList.contains("create-button")) {
    await sendForm(
      async (form, data) => await post(form.dataset.endpoint, data),
      e
    );
    return;
  }

  if (e.target.classList.contains("search-button")) {
    await updateTable(e.target);
    return;
  }

  if (e.target.classList.contains("create")) {
    main.innerHTML = await makeForm(e.target, "Создать", "create-button");
    return;
  }

  if (e.target.tagName === "TD" && e.target.closest(".body") !== null) {
    const id = e.target.parentElement.querySelector("td").textContent;

    if (pageSnapshot) {
      main.innerHTML = pageSnapshot;
      pageSnapshot = null;
      const linkToTable = document.querySelector(".link-to-table");
      linkToTable.setAttribute("value", id);
      linkToTable.textContent =
        e.target.parentElement.querySelectorAll("td")[1].textContent;
      return;
    }

    const values = Array.from(e.target.parentElement.querySelectorAll("td"))
      .map((x) => x.textContent)
      .slice(1);
    console.log(values);

    main.innerHTML = await makeForm(
      e.target,
      "Обновить",
      "update-button",
      values,
      id
    );
    return;
  }
});

async function handleNavigationClick(e) {
  const endpoint = e.target.dataset.endpoint;
  if (endpoint) {
    const title = e.target.textContent;
    main.innerHTML = await makeTable(title, endpoint);
  }
}
