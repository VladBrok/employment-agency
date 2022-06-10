import columns from "./columns.js";
import { post } from "./api.js";
import { fetchJsonFromTable } from "./table.js";

async function downloadReport(tableChild, type) {
  let data = await fetchJsonFromTable({
    tableChild,
    pageSize: 1e6,
  });
  data = await convertValues(data.slice(0, 5));
  data[0] = convertColumnNames(data[0]);

  const fileName = `report.${type === "excel" ? "xlsx" : type}`;
  const title = tableChild
    .closest("[data-endpoint]")
    .querySelector(".title").textContent;
  const report = await post(
    `/reports/${type}?fileName=${fileName}&title=${title}`,
    JSON.stringify(data)
  );

  download(await report.blob(), fileName);
}

function convertColumnNames(obj) {
  let result = {};
  Object.keys(obj).forEach(
    (key) => (result = { ...result, [columns[key].displayName]: obj[key] })
  );
  return result;
}

async function convertValues(data) {
  return await Promise.all(
    data.map(async (d) =>
      Object.fromEntries(
        await Promise.all(
          Object.entries(d).map(async ([name, value], _, entries) => [
            name,
            await columns[name].convertValue(value, entries),
          ])
        )
      )
    )
  );
}

function download(reportBlob, fileName) {
  const link = document.createElement("a");
  link.download = fileName;
  link.href = URL.createObjectURL(reportBlob);
  link.click();
  URL.revokeObjectURL(link.href);
}

export default downloadReport;
