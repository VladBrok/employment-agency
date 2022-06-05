import { post } from "./api.js";
import loadingDecorator from "./loading.js";

const form = document.querySelector(".crud-form");
const login = document.getElementById("login");
const password = document.getElementById("password");

document.addEventListener("input", handleInput);
form.onsubmit = loadingDecorator(handleSubmit);

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
