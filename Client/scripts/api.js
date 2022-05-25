const URL = "https://localhost:5001/api";
const PAGE_SIZE = 15;

async function fetchJson({
  endpoint,
  page = 0,
  filter = null,
  parameters = null,
}) {
  const response = await fetch(
    `${URL}${
      parameters ? `${endpoint}/${parameters.join("/")}` : endpoint
    }?page=${+page}&pageSize=${PAGE_SIZE}${filter ? `&filter=${filter}` : ""}`
  );
  return await response.json();
}

export default fetchJson;
