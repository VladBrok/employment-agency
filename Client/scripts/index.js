import { makeTable, updateTable } from "./table.js";
import { makeForm, sendFormAsPost, sendFormAsPut } from "./form.js";
import downloadReport from "./report.js";
import confirmDelete from "./confirm.js";
import { deleteEntity } from "./api.js";

const navigation = document.querySelector(".navigation");
const main = document.querySelector(".main");

navigation.addEventListener("click", handleNavigationClick);
main.addEventListener("change", async (e) => {
  if (e.target.classList.contains("download")) {
    const type = e.target.value;
    e.target.selectedIndex = 0;

    await downloadReport(e.target, type);
    return;
  }

  if (e.target.files) {
    // TODO: check an extension
    main.querySelector(".photo").src = e.target.files[0]
      ? URL.createObjectURL(e.target.files[0])
      : "";
  }
});

let choosing = false;

main.addEventListener("click", async (e) => {
  if (e.target.classList.contains("link-to-table")) {
    const endpoint = e.target.dataset.endpoint;
    main.querySelector(".compound-form").style.visibility = "hidden";
    main.insertAdjacentHTML("afterbegin", await makeTable(endpoint));
    choosing = true;
    return;
  }

  if (e.target.classList.contains("previous-page")) {
    const page = e.target.parentElement.querySelector(".current-page");
    page.textContent--;
    if (+page.textContent === 1) {
      e.target.classList.add("disabled");
    }

    await updateTable(page, page.textContent - 1);
    return;
  }

  if (e.target.classList.contains("next-page")) {
    const page = e.target.parentElement.querySelector(".current-page");
    page.textContent++;
    if (+page.textContent === 2) {
      e.target.parentElement
        .querySelector(".previous-page")
        .classList.remove("disabled");
    }

    await updateTable(page, page.textContent - 1);
    return;
  }

  if (e.target.classList.contains("update-button")) {
    await sendFormAsPut(e);
    return;
  }

  if (e.target.classList.contains("create-button")) {
    await sendFormAsPost(e);
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

  if (e.target.classList.contains("delete")) {
    const confirmed = await confirmDelete();
    if (confirmed) {
      await deleteEntity(
        main.querySelector("[data-endpoint]").dataset.endpoint,
        main.querySelector("[data-id]").dataset.id
      );
    }
    return;
  }

  if (e.target.tagName === "TD" && e.target.closest(".body") !== null) {
    const id = e.target.parentElement.querySelector("td").textContent;

    if (choosing) {
      main.querySelector(".table-container").remove();
      main.querySelector(".compound-form").style.visibility = "visible";
      choosing = false;
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
  const selected = navigation.querySelector(".selected-item");
  const newSelected =
    e.target.closest(".item.parent") ?? e.target.closest(".item");
  selected.classList.remove("selected-item");
  newSelected.classList.add("selected-item");

  const endpoint = e.target.dataset.endpoint;
  if (endpoint) {
    const title = e.target.textContent;
    main.innerHTML = await makeTable(endpoint, null, title);
  }
}
