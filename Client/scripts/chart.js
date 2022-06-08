import "../node_modules/highcharts/es-modules/masters/highcharts.src.js";
import "../node_modules/highcharts/es-modules/masters/highcharts-3d.src.js";
import Highcharts from "../node_modules/highcharts/es-modules/Core/Globals.js";
import { fetchJsonFromTable } from "./table.js";

const DEPTH = 250;
const CHART_OPTIONS = {
  "1d": (data) => ({
    tooltip: {
      pointFormat: "{series.name}: <b>{point.percentage:.2f}%</b>",
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
  }),
  "2d": (data) => ({
    chart: {
      type: "column",
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
    series: data.map((d) => ({
      name: d["Должность"],
      data: [+d["Средняя зарплата"]],
    })),
  }),
  "3d": (data) => ({
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
  }),
};

export default async function drawChart(tableChild, type, container) {
  const data = await fetchJsonFromTable({
    tableChild,
    pageSize: 1e6,
  });
  const title = tableChild
    .closest("[data-endpoint]")
    .querySelector(".title").textContent;

  Highcharts.chart(container, {
    ...CHART_OPTIONS[type](data),
    title: {
      text: title,
    },
  });

  document.querySelector(".highcharts-credits").remove();
}
