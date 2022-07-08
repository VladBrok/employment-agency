import { makeTable } from "./table.js";
import { makeForm, sendFormAsPost, sendFormAsPut } from "./form.js";
import { exit } from "./auth.js";
import fetchJson, { deleteEntity } from "./api.js";
import { confirm, showInfo } from "./modal-windows.js";
import downloadReport from "./report.js";
import loadingDecorator from "./loading.js";
import drawChart from "./chart.js";

const navigation = document.querySelector(".navigation");
const main = document.querySelector(".main");

focusOnMenu();

window.addEventListener("unhandledrejection", handleError);
navigation.addEventListener("click", loadingDecorator(handleNavigationClick));
document.addEventListener("click", loadingDecorator(handleDocumentClick));
main.addEventListener("change", loadingDecorator(handleChange));

function focusOnMenu() {
  document.querySelector(".menu-wrapper").focus();
}

function handleError() {
  main.innerHTML = `
    <div class='error-container'>
      <span class='title'>
        Не удалось подключиться к серверу. Пожалуйста, повторите попытку позже.
      </span>
    </div>`;
}

let displayTable;

async function handleNavigationClick(e) {
  const subMenu = e.target.nextElementSibling;
  if (subMenu) {
    if (subMenu.style.display === "block") {
      for (const child of subMenu.querySelectorAll(".child")) {
        child.style.display = "none";
      }
      subMenu.style.display = "none";
    } else {
      subMenu.style.display = "block";
      setTimeout(() => subMenu.querySelector("a")?.scrollIntoView(false));
    }
    return;
  }

  if (!e.target.dataset.endpoint) {
    return;
  }

  const selected = navigation.querySelector(".selected-item");
  const newSelected =
    e.target.closest(".item.parent") ?? e.target.closest(".item");
  selected.classList.remove("selected-item");
  newSelected.classList.add("selected-item");
  const endpoint = e.target.dataset.endpoint;

  if (e.target.classList.contains("generate-data")) {
    const confirmed = await confirm(
      "Таблицы будут заполнены случайными данными. Текущие данные будут потеряны.",
      "Генерация"
    );
    if (confirmed) {
      await fetchJson({ endpoint });
      showMessage("Данные сгенерированы!");
    }
    return;
  }

  const chartType = e.target.dataset.chart;
  const title = e.target.textContent;
  displayTable = async () =>
    (main.innerHTML = await makeTable({ endpoint, chartType, title }));
  await displayTable();

  if (document.documentElement.offsetWidth <= 700) {
    toggleMenu();
  }
}

let choosing = false;
const showMessage = showInfo.bind(null, () => displayTable());

async function handleDocumentClick(e) {
  if (e.target.classList.contains("delete-many")) {
    if (!main.querySelector(".table .body [type=checkbox]")) {
      [...main.querySelectorAll(".table tr")].forEach((row) => {
        row.classList.add("delete-row");
        row.insertAdjacentHTML(
          "afterbegin",
          '<td><input type="checkbox"></td>'
        );
      });
      return;
    }

    const checked = [
      ...main.querySelectorAll(".table .body [type=checkbox]"),
    ].filter((x) => x.checked);
    if (!checked.length) {
      await displayTable();
      return;
    }

    const confirmed = await confirm(
      `Все выбранные записи (${checked.length}) и зависимые от них данные будут удалены. Данное действие нельзя отменить.`,
      "Удалить"
    );
    if (!confirmed) {
      await displayTable();
      return;
    }

    await Promise.all(
      checked.map(
        async (c) =>
          await deleteEntity(
            e.target.closest("[data-endpoint]").dataset.endpoint,
            +c.parentElement.nextElementSibling.textContent
          )
      )
    );
    showMessage("Данные удалены!");
    return;
  }

  if (e.target.classList.contains("menu-wrapper")) {
    toggleMenu();
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

  if (
    e.target.tagName === "INPUT" &&
    e.target.closest(".head") !== null &&
    e.target.checked
  ) {
    [...main.querySelectorAll(".table .body [type=checkbox]")].forEach(
      (c) => (c.checked = true)
    );
    return;
  }

  if (e.target.tagName === "TD" && e.target.closest(".body") !== null) {
    const checkbox = e.target.parentElement.querySelector("[type=checkbox]");
    if (checkbox) {
      checkbox.checked = !checkbox.checked;
      return;
    }

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
    const allowedExtensions = new Set([".jpg", ".png", ".jpeg"]);
    const file = e.target.files[0];
    const extension = file?.name.slice(file.name.indexOf("."));

    if (extension && !allowedExtensions.has(extension)) {
      e.target.setCustomValidity(
        `Файл должен иметь один из следующих форматов: ${[
          ...allowedExtensions.values(),
        ].join(", ")}`
      );
      e.target.reportValidity();
      return;
    }
    main.querySelector(".photo").src = file ? URL.createObjectURL(file) : "#";
  }
}

function toggleMenu() {
  const toggle = () => {
    navigation.classList.toggle("show");
    document.querySelector(".menu-wrapper").outerHTML = `${
      !navigation.classList.contains("show")
        ? `<button class="menu-wrapper"><img src="images/menu.png" alt="Открыть меню" class="menu" title="Открыть меню"></button>`
        : `<button class="menu-wrapper"><img src="images/close-menu.png" alt="Закрыть меню" class="menu close-menu" title="Закрыть меню"></button>`
    }`;
    focusOnMenu();
  };

  if (navigation.classList.contains("show")) {
    navigation.ontransitionend = () => {
      navigation.hidden = true;
      navigation.ontransitionend = null;
    };
    toggle();
  } else {
    navigation.hidden = false;
    setTimeout(toggle);
  }
}
