import columns from "./columns.js";
import { fetchAllJson, post } from "./api.js";

// Параметры не учитываются !
async function downloadReport({ type, endpoint, title }) {
  const data = await fetchAllJson(endpoint);
  let first = {};
  Object.keys(data[0]).forEach(
    (k) => (first = { ...first, [columns[k]?.displayName ?? k]: data[0][k] })
  );
  data[0] = first;

  const fileName = `report.${type}`;
  const report = await post(
    `/reports/${type}?fileName=${fileName}&title=${title}`,
    JSON.stringify(data)
  );

  const link = document.createElement("a");
  link.download = fileName;
  link.href = URL.createObjectURL(await report.blob());
  link.click();
  URL.revokeObjectURL(link.href);
}

export default downloadReport;
