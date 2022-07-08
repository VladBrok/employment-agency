const loginPage = { element: document.getElementById("login"), name: "login" };
const indexPage = { element: document.getElementById("index"), name: "index" };
const pagePlaceholder = document.querySelector(".template-placeholder");
let curPage = null;

async function ensureAuthenticated() {
  if (!getToken() || tokenHasExpired()) {
    await goToLoginPage();
  } else {
    await goToIndexPage();
  }
}

function getToken() {
  return localStorage.getItem("token");
}

function tokenHasExpired() {
  return Date.now() - localStorage.getItem("expires") >= -600000;
}

async function goToLoginPage({ clearAll = false } = {}) {
  localStorage.removeItem("token");
  localStorage.removeItem("expires");
  if (clearAll) {
    sessionStorage.removeItem("was_authenticated");
  }
  changePageTo(loginPage);
  const { initialize } = await import("./login.js");
  initialize();
}

async function goToIndexPage() {
  changePageTo(indexPage);
  await import("./index.js");
}

function authenticate(token, expirationTime) {
  localStorage.setItem("token", token);
  localStorage.setItem("expires", expirationTime);
  sessionStorage.setItem("was_authenticated", "yes");
  changePageTo(indexPage);
}

function changePageTo(newPage) {
  if (curPage === newPage.name) {
    return;
  }

  pagePlaceholder.innerHTML = "";
  pagePlaceholder.append(newPage.element.content.cloneNode(true));
  curPage = newPage.name;
}

function exit() {
  goToLoginPage({ clearAll: true });
}

function wasAuthenticated() {
  return Boolean(sessionStorage.getItem("was_authenticated"));
}

export { ensureAuthenticated, authenticate, exit, getToken, wasAuthenticated };
