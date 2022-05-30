import columns from "./columns.js";
import { fetchJsonFromTable, post } from "./api.js";

async function downloadReport(tableChild, type) {
  const data = await fetchJsonFromTable({
    tableChild,
    pageSize: 1e6,
  });
  let first = {};
  Object.keys(data[0]).forEach(
    (k) => (first = { ...first, [columns[k]?.displayName ?? k]: data[0][k] })
  );
  data[0] = first;

  const fileName = `report.${type}`;
  const report = await post(
    `/reports/${type}?fileName=${fileName}&title=${
      tableChild.closest("[data-endpoint]").querySelector(".title").textContent
    }`,
    JSON.stringify(data)
  );

  const link = document.createElement("a");
  link.download = fileName;
  link.href = URL.createObjectURL(await report.blob());
  link.click();
  URL.revokeObjectURL(link.href);
}

export default downloadReport;
