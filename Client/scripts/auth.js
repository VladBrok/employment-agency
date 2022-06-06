function ensureAuthenticated() {
  if (!localStorage.getItem("token") || tokenHasExpired()) {
    location.replace("./login.html");
  }
}

function authenticate(token, expirationTime) {
  localStorage.setItem("token", token);
  localStorage.setItem("expires", expirationTime);
  sessionStorage.setItem("was_authenticated", "yes");
  location.replace("./index.html");
}

function exit() {
  removeTokenInfo();
  sessionStorage.removeItem("was_authenticated");
  location.replace("./login.html");
}

function getToken() {
  return localStorage.getItem("token");
}

function wasAuthenticated() {
  return Boolean(sessionStorage.getItem("was_authenticated"));
}

function ensureTokenValid() {
  if (localStorage.getItem("expires") && tokenHasExpired()) {
    removeTokenInfo();
    location.replace("./login.html");
  }
}

function tokenHasExpired() {
  return Date.now() - localStorage.getItem("expires") >= 600000;
}

function removeTokenInfo() {
  localStorage.removeItem("token");
  localStorage.removeItem("expires");
}

export {
  ensureAuthenticated,
  authenticate,
  exit,
  getToken,
  wasAuthenticated,
  ensureTokenValid,
};
