const URL = "https://localhost:7288/api";
const PAGE_SIZE = 15;

async function fetchJson({
  endpoint,
  page = 0,
  filter = null,
  parameterValues = [],
  pageSize = PAGE_SIZE,
}) {
  const response = await fetch(
    `${URL}${`${endpoint}/${parameterValues.join(
      "/"
    )}`}?page=${+page}&pageSize=${pageSize}${filter ? `&filter=${filter}` : ""}`
  );

  if (response.status === 404) {
    return [];
  }
  return await response.json();
}

async function fetchAllJson(endpoint) {
  return await fetchJson({ endpoint, page: 0, pageSize: 1e6 });
}

export { fetchAllJson };
export default fetchJson;
