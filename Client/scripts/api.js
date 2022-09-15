import { ensureAuthenticated, getToken } from "./auth.js";
import { apiUrls, isDev } from "./config.js";

const PAGE_SIZE = 15;
const TIMEOUT_IN_MILLISECONDS = isDev ? 1e6 : 30000;
let urlIndex = 0;

async function fetchJson({
  endpoint,
  page = 0,
  filter = null,
  parameterValues = [],
  pageSize = PAGE_SIZE,
}) {
  const response = await fetchImpl(
    `${`${endpoint}/${parameterValues.join(
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
  await fetchImpl(`${endpoint}/${id}`, {
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
  return await fetchImpl(endpoint, options, anonymous);
}

async function deleteEntity(endpoint, id) {
  return await fetchImpl(`${endpoint}/${id}`, { method: "DELETE" });
}

async function fetchImpl(url, options, anonymous = false) {
  if (!anonymous) {
    await ensureAuthenticated();
  }

  const controller = new AbortController();
  const signal = controller.signal;
  const adjustedOptions = {
    ...options,
    signal,
    headers: {
      ...options?.headers,
      Authorization: `Bearer ${getToken()}`,
    },
  };

  setTimeout(() => controller.abort(), TIMEOUT_IN_MILLISECONDS);
  let response;
  try {
    response = await fetch(makeUrl(url), adjustedOptions);
    return response;
  } catch (er) {
    if (
      (er instanceof DOMException || er instanceof TypeError) &&
      urlIndex < apiUrls.length - 1
    ) {
      urlIndex++;
      console.log("New url index:", urlIndex);
      return await fetchImpl(url, options, anonymous);
    }
    throw er;
  } finally {
    console.log(response?.status);
  }
}

function makeUrl(endpoint) {
  return `${apiUrls[urlIndex]}${endpoint}`;
}

function makeImageUrl(imageName) {
  return makeUrl(`/photos/${imageName}`);
}

export { fetchAllJson, put, post, deleteEntity, PAGE_SIZE, makeImageUrl };
export default fetchJson;
