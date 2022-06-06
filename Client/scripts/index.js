import { makeTable, updateTable } from "./table.js";
import { makeForm, sendFormAsPost, sendFormAsPut } from "./form.js";
import { ensureAuthenticated, exit } from "./auth.js";
import fetchJson, { deleteEntity } from "./api.js";
import { confirm, showInfo } from "./modal-windows.js";
import downloadReport from "./report.js";
import loadingDecorator from "./loading.js";
import drawChart from "./chart.js";

ensureAuthenticated();

const navigation = document.querySelector(".navigation");
const main = document.querySelector(".main");

navigation.addEventListener("click", loadingDecorator(handleNavigationClick));
document.addEventListener("click", loadingDecorator(handleDocumentClick));
main.addEventListener("change", loadingDecorator(handleChange));

let displayTable;

async function handleNavigationClick(e) {
  if (!e.target.dataset.endpoint) {
    return;
  }

  const selected = navigation.querySelector(".selected-item");
  const newSelected =
    e.target.closest(".item.parent") ?? e.target.closest(".item");
  selected.classList.remove("selected-item");
  newSelected.classList.add("selected-item");

  if (e.target.dataset.category === "not-a-table") {
    return;
  }

  const endpoint = e.target.dataset.endpoint;
  const chartType = e.target.dataset.chart;
  const title = e.target.textContent;
  displayTable = async () =>
    (main.innerHTML = await makeTable({ endpoint, chartType, title }));
  displayTable();
}

let choosing = false;
const showMessage = showInfo.bind(null, () => displayTable());

async function handleDocumentClick(e) {
  if (e.target.classList.contains("menu")) {
    navigation.classList.toggle("hidden");
    document.querySelector(".menu").outerHTML = navigation.classList.contains(
      "hidden"
    )
      ? `<img src="images/menu.png" alt="Открыть меню" class="menu" title="Открыть меню">`
      : `<img src="images/close-menu.png" alt="Закрыть меню" class="menu close-menu" title="Закрыть меню">`;
    return;
  }

  if (e.target.classList.contains("exit-button")) {
    exit();
    return;
  }

  if (e.target.classList.contains("link-to-table")) {
    const endpoint = e.target.dataset.endpoint;
    main.querySelector(".compound-form").style.visibility = "hidden";
    main.insertAdjacentHTML("afterbegin", await makeTable({ endpoint }));
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
    if (await sendFormAsPut(e)) {
      showMessage("Данные обновлены!");
    }
    return;
  }

  if (e.target.classList.contains("create-button")) {
    if (await sendFormAsPost(e)) {
      showMessage("Новая запись создана!");
    }
    return;
  }

  if (e.target.classList.contains("find")) {
    await updateTable(e.target);
    return;
  }

  if (e.target.classList.contains("create")) {
    main.innerHTML = await makeForm(e.target, "Создать", "create-button");
    return;
  }

  if (e.target.classList.contains("chart")) {
    await drawChart(e.target, e.target.dataset.chart, main);
    return;
  }

  if (e.target.classList.contains("delete")) {
    const confirmed = await confirm(
      "Все зависимые записи также удалятся. Данное действие нельзя отменить.",
      "Удалить"
    );
    if (confirmed) {
      await deleteEntity(
        main.querySelector("[data-endpoint]").dataset.endpoint,
        main.querySelector("[data-id]").dataset.id
      );
      showMessage("Данные удалены!");
    }
    return;
  }

  if (e.target.classList.contains("generate-data")) {
    const confirmed = await confirm(
      "Таблицы будут заполнены заранее определенными данными. Текущие данные будут потеряны.",
      "Генерация"
    );
    if (confirmed) {
      await fetchJson({ endpoint: e.target.dataset.endpoint });
      showMessage("Данные сгенерированы!");
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
    main.innerHTML = await makeForm(
      e.target,
      "Обновить",
      "update-button",
      values,
      id
    );
    return;
  }
}

async function handleChange(e) {
  if (e.target.classList.contains("download")) {
    const reportType = e.target.value;
    e.target.selectedIndex = 0;

    await downloadReport(e.target, reportType);
    return;
  }

  if (e.target.files) {
    const ALLOWED_EXTENSIONS = new Set([".jpg", ".png", ".jpeg"]);
    const file = e.target.files[0];
    const extension = file?.name.slice(file.name.indexOf("."));

    if (extension && !ALLOWED_EXTENSIONS.has(extension)) {
      e.target.setCustomValidity(
        `Файл должен иметь один из следующих форматов: ${[
          ...ALLOWED_EXTENSIONS.values(),
        ].join(", ")}`
      );
      e.target.reportValidity();
      return;
    }
    main.querySelector(".photo").src = file ? URL.createObjectURL(file) : "#";
  }
}
