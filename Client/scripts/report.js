import columns from "./columns.js";
import { fetchJsonFromTable, post } from "./api.js";

async function downloadReport(tableChild, type) {
  const data = await fetchJsonFromTable({
    tableChild,
    pageSize: 1e6,
  });
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

function download(reportBlob, fileName) {
  const link = document.createElement("a");
  link.download = fileName;
  link.href = URL.createObjectURL(reportBlob);
  link.click();
  URL.revokeObjectURL(link.href);
}

export default downloadReport;
