import { makeTable, updateTable } from "./table.js";
import { makeForm, sendFormAsPost, sendFormAsPut } from "./form.js";
import downloadReport from "./report.js";
import confirmDelete from "./confirm.js";
import { deleteEntity, fetchJsonFromTable } from "./api.js";
import "../node_modules/highcharts/es-modules/masters/highcharts.src.js";
import "../node_modules/highcharts/es-modules/masters/highcharts-3d.src.js";
import Highcharts from "../node_modules/highcharts/es-modules/Core/Globals.js";

if (!localStorage.getItem("token")) {
  location.replace("./login.html");
}

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

document.addEventListener("click", async (e) => {
  if (e.target.classList.contains("menu")) {
    navigation.classList.toggle("hidden");
    document.querySelector(".menu").outerHTML = navigation.classList.contains(
      "hidden"
    )
      ? `<img src="images/menu.png" alt="Открыть меню" class="menu" title="Открыть меню">`
      : `<img src="images/close-menu.png" alt="Закрыть меню" class="menu close-menu" title="Закрыть меню">`;
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

  if (e.target.classList.contains("chart")) {
    const data = await fetchJsonFromTable({
      tableChild: e.target,
      pageSize: 1e6,
    });
    console.log(data);

    const chartType = e.target.dataset.chart;
    const title = e.target
      .closest("[data-endpoint]")
      .querySelector(".title").textContent;

    // TODO: change
    switch (chartType) {
      case "1d":
        Highcharts.chart(main, {
          title: {
            text: title,
          },
          tooltip: {
            pointFormat: "{series.name}: <b>{point.percentage:.2f}%</b>",
          },
          plotOptions: {
            pie: {
              cursor: "pointer",
            },
          },
          series: [
            {
              type: "pie",
              name: "% заявок",
              data: data.map((d) => [
                d["Должность"],
                +d["% заявок"].replace(",", "."),
              ]),
            },
          ],
        });
        break;
      case "2d":
        Highcharts.chart(main, {
          chart: {
            type: "column",
          },
          title: {
            text: title,
          },
          xAxis: {
            visible: false,
          },
          yAxis: {
            title: {
              text: "Зарплата",
            },
          },
          tooltip: {
            headerFormat: "<span></span>",
          },
          plotOptions: {
            column: {
              pointPadding: 0.2,
              borderWidth: 0,
            },
          },
          series: data.map((d) => ({
            name: d["Должность"],
            data: [+d["Средняя зарплата"]],
          })),
        });
        break;
      case "3d":
        const DEPTH = 250;
        Highcharts.chart(main, {
          chart: {
            type: "column",
            options3d: {
              enabled: true,
              alpha: 15,
              beta: 15,
              depth: DEPTH,
              viewDistance: 25,
            },
          },
          yAxis: {
            title: {
              text: "Опыт",
            },
          },
          xAxis: {
            title: {
              text: "Зарплата",
            },
          },
          title: {
            text: title,
          },
          plotOptions: {
            column: {
              depth: DEPTH,
              pointPadding: 0,
              borderWidth: 0,
              groupPadding: -1,
            },
          },
          series: data.map((d) => ({
            name: d["Тип занятости"],
            data: [[+d["Средняя зарплата"], +d["Средний опыт"]]],
          })),
        });
        break;
    }
    document.querySelector(".highcharts-credits").remove();
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
  if (!e.target.dataset.endpoint) {
    return;
  }

  const selected = navigation.querySelector(".selected-item");
  const newSelected =
    e.target.closest(".item.parent") ?? e.target.closest(".item");
  selected.classList.remove("selected-item");
  newSelected.classList.add("selected-item");

  const endpoint = e.target.dataset.endpoint;
  const chartType = e.target.dataset.chart;
  const title = e.target.textContent;
  main.innerHTML = await makeTable({ endpoint, title, chartType });
}
