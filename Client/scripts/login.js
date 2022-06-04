import { post } from "./api.js";

const form = document.querySelector(".crud-form");
const login = document.getElementById("login");
const password = document.getElementById("password");

document.addEventListener("input", (e) => {
  e.target.setCustomValidity("");
});

form.onsubmit = async (e) => {
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

  const input = response.error.indexOf("логин") === -1 ? password : login;
  setInvalid(input, response.error);
};

function setInvalid(input, errorMessage) {
  input.setCustomValidity(errorMessage);
  input.reportValidity();
}
