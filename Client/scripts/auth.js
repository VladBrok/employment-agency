function ensureAuthenticated() {
  if (!getToken() || tokenHasExpired()) {
    goToLoginPage();
  }
}

function getToken() {
  return localStorage.getItem("token");
}

function tokenHasExpired() {
  return Date.now() - localStorage.getItem("expires") >= -600000;
}

function goToLoginPage({ clearAll = false } = {}) {
  localStorage.removeItem("token");
  localStorage.removeItem("expires");
  if (clearAll) {
    sessionStorage.removeItem("was_authenticated");
  }
  location.replace("./login.html");
}

function authenticate(token, expirationTime) {
  localStorage.setItem("token", token);
  localStorage.setItem("expires", expirationTime);
  sessionStorage.setItem("was_authenticated", "yes");
  location.replace("./index.html");
}

function exit() {
  goToLoginPage({ clearAll: true });
}

function wasAuthenticated() {
  return Boolean(sessionStorage.getItem("was_authenticated"));
}

export { ensureAuthenticated, authenticate, exit, getToken, wasAuthenticated };
