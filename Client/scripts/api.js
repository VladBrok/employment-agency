const URL = "https://localhost:7288/api";
const PAGE_SIZE = 15;

async function fetchJson({
  endpoint,
  page = 0,
  filter = null,
  parameters = null,
  pageSize = PAGE_SIZE,
}) {
  const response = await fetch(
    `${URL}${
      parameters ? `${endpoint}/${parameters.join("/")}` : endpoint
    }?page=${+page}&pageSize=${pageSize}${filter ? `&filter=${filter}` : ""}`
  );
  return await response.json();
}

async function fetchAllJson(endpoint) {
  return await fetchJson({ endpoint, page: 0, pageSize: 1e6 });
}

export { fetchAllJson };
export default fetchJson;
