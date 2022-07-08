import { ensureAuthenticated, getToken } from "./auth.js";
import { apiUrl } from "./config.js";

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
  let json;
  try {
    json = await response.json();
  } catch (err) {
    return [];
  }
  return json;
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

async function post(endpoint, data, anonymous = false) {
  const options = {
    method: "POST",
    body: data,
  };
  if (!(data instanceof FormData)) {
    options.headers = {
      "Content-Type": "application/json",
    };
  }
  return await fetchImpl(makeUrl(endpoint), options, anonymous);
}

async function deleteEntity(endpoint, id) {
  return await fetchImpl(`${makeUrl(endpoint)}/${id}`, { method: "DELETE" });
}

function makeUrl(endpoint) {
  return `${apiUrl}${endpoint}`;
}

async function fetchImpl(url, options, anonymous = false) {
  if (!anonymous) {
    ensureAuthenticated();
  }

  options = {
    ...options,
    headers: {
      ...options?.headers,
      Authorization: `Bearer ${getToken()}`,
    },
  };

  const response = await fetch(url, options);
  console.log(response.status);
  return response;
}

function makeImageUrl(imageName) {
  return `${apiUrl}/photos/${imageName}`;
}

export { fetchAllJson, put, post, deleteEntity, PAGE_SIZE, makeImageUrl };
export default fetchJson;
