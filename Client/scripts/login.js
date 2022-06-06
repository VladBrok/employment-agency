import { post } from "./api.js";
import loadingDecorator from "./loading.js";

adjustTitle();
const form = document.querySelector(".crud-form");
const login = document.getElementById("login");
const password = document.getElementById("password");

document.addEventListener("input", handleInput);
form.onsubmit = loadingDecorator(handleSubmit);

function adjustTitle() {
  if (sessionStorage.getItem("was_here")) {
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
  const response = await (await post("/login", JSON.stringify(user))).json();

  if (!response.error) {
    localStorage.setItem("token", response.access_token);
    localStorage.setItem("expires", response.expires);
    sessionStorage.setItem("was_here", "yes");
    location.replace("./index.html");
    return;
  }

  const input = response.error.includes("логин") ? login : password;
  setInvalid(input, response.error);
}

function setInvalid(input, errorMessage) {
  input.setCustomValidity(errorMessage);
  input.reportValidity();
}
