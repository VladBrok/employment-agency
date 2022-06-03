const form = document.querySelector(".crud-form");
const login = document.getElementById("login");
const password = document.getElementById("password");

document.addEventListener("input", (e) => {
  e.target.setCustomValidity("");
});

form.onsubmit = (e) => {
  e.preventDefault();

  if (validate(login, "admin", "Неверный логин", e)) {
    if (validate(password, "1234", "Неверный пароль", e)) {
      sessionStorage.setItem("logged_in", "yes");
      location.replace("./index.html");
    }
  }
};

function validate(input, expected, errorMessage) {
  if (input.value !== expected) {
    input.setCustomValidity(errorMessage);
    input.reportValidity();
    return false;
  }
  return true;
}
