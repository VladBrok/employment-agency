import { post } from "./api.js";
import { authenticate, wasAuthenticated } from "./auth.js";
import loadingDecorator from "./loading.js";

let form, login, password;

function initialize() {
  adjustTitle();

  form = document.querySelector(".crud-form");
  login = document.getElementById("login-input");
  password = document.getElementById("password");

  document.addEventListener("input", handleInput);
  form.onsubmit = loadingDecorator(handleSubmit);
  login.focus();
}

function dispose() {
  form = login = password = null;
  document.removeEventListener("input", handleInput);
}

function adjustTitle() {
  if (wasAuthenticated()) {
    document.querySelector(".title").innerHTML =
      "Время сеанса истекло.<br>Пожалуйста, авторизуйтесь снова";
  }
}

function handleInput(e) {
  e.target.setCustomValidity("");
}

async function handleSubmit(e) {
  e.preventDefault();

  const user = {
    login: login.value,
    password: password.value,
  };
  const anonymous = true;
  const response = await (
    await post("/login", JSON.stringify(user), anonymous)
  ).json();

  if (!response.error) {
    authenticate(response.access_token, response.expires);
    dispose();
    window.location.reload();
    return;
  }

  const input = response.error.includes("логин") ? login : password;
  setInvalid(input, response.error);
}

function setInvalid(input, errorMessage) {
  input.setCustomValidity(errorMessage);
  input.reportValidity();
}

export { initialize };
