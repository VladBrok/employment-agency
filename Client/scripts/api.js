const URL = "https://localhost:7288/api";
const PAGE_SIZE = 15;

async function fetchJson({
  endpoint,
  page = 0,
  filter = null,
  parameterValues = [],
  pageSize = PAGE_SIZE,
}) {
  const response = await fetchImpl(
    `${`${makeUrl(endpoint)}/${parameterValues.join(
      "/"
    )}`}?page=${+page}&pageSize=${pageSize}${filter ? `&filter=${filter}` : ""}`
  );

  if (response.status === 404) {
    return [];
  }
  return await response.json();
}

async function fetchBlob(endpoint) {
  const response = await fetchImpl(makeUrl(endpoint));
  return await response.blob();
}

async function fetchJsonFromTable({
  tableChild,
  page = 0,
  pageSize = PAGE_SIZE,
}) {
  const tableContainer = tableChild.closest("[data-endpoint]");
  const endpoint = tableContainer.dataset.endpoint;

  const inputs = Array.from(tableContainer.querySelectorAll(".input"));
  const filter = inputs[0].value;
  const parameterValues = inputs.slice(1).map((x) => x.value);

  return await fetchJson({
    endpoint,
    page,
    filter,
    parameterValues,
    pageSize,
  });
}

async function fetchAllJson(endpoint) {
  return await fetchJson({ endpoint, page: 0, pageSize: 1e6 });
}

async function put(endpoint, id, formData) {
  await fetchImpl(`${makeUrl(endpoint)}/${id}`, {
    method: "PUT",
    body: formData,
  });
}

async function post(endpoint, data) {
  const options = {
    method: "POST",
    body: data,
  };
  if (!(data instanceof FormData)) {
    options.headers = {
      "Content-Type": "application/json",
    };
  }
  return await fetchImpl(makeUrl(endpoint), options);
}

async function deleteEntity(endpoint, id) {
  return await fetchImpl(`${makeUrl(endpoint)}/${id}`, { method: "DELETE" });
}

function makeUrl(endpoint) {
  return `${URL}${endpoint}`;
}

async function fetchImpl(url, options) {
  options = {
    ...options,
    headers: {
      ...options?.headers,
      Authorization: `Bearer ${localStorage.getItem("token")}`,
    },
  };
  const response = await fetch(url, options);
  console.log(response.status);
  return response;
}

export { fetchJsonFromTable, fetchBlob, fetchAllJson, put, post, deleteEntity };
export default fetchJson;
