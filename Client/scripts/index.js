import { makeTable, updateTable } from "./table.js";
import makeForm from "./form.js";
import columns from "./columns.js";
import { put } from "./api.js";

const navigation = document.querySelector(".navigation");
const main = document.querySelector(".main");

navigation.addEventListener("click", async (e) => {
  const endpoint = e.target.dataset.endpoint;

  if (endpoint) {
    const title = e.target.textContent;
    main.innerHTML = await makeTable(title, endpoint);
  }
});

main.addEventListener("click", async (e) => {
  if (e.target.classList.contains("previous-page")) {
    const page = e.target.parentElement.querySelector(".current-page");
    page.textContent--;

    await updateTable(page, page.textContent - 1);

    if (+page.textContent === 1) {
      e.target.classList.add("disabled");
    }
    return;
  }

  if (e.target.classList.contains("next-page")) {
    const page = e.target.parentElement.querySelector(".current-page");
    page.textContent++;

    await updateTable(page, page.textContent - 1);

    if (+page.textContent === 2) {
      e.target.parentElement
        .querySelector(".previous-page")
        .classList.remove("disabled");
    }
    return;
  }

  if (e.target.classList.contains("update-button")) {
    e.preventDefault();
    await update();
    return;
  }

  if (e.target.classList.contains("search-button")) {
    await updateTable(e.target);
    return;
  }

  if (e.target.classList.contains("create")) {
    main.innerHTML = await makeForm(e.target, "Создать", "create-button");
    return;
  }

  if (e.target.tagName === "TD" && e.target.closest(".body") !== null) {
    const values = Array.from(e.target.parentElement.querySelectorAll("td"))
      .map((x) => x.textContent)
      .slice(1);
    console.log(values);

    main.innerHTML = await makeForm(
      e.target,
      "Обновить",
      "update-button",
      values,
      e.target.parentElement.querySelector("td").textContent
    );
    return;
  }
});

async function update() {
  const form = document.querySelector(".crud-form");
  const names = [];
  const values = [];

  for (const input of form.querySelectorAll(".input, input:checked")) {
    console.log(input);
    if (input.checkValidity && !input.checkValidity()) {
      return;
    }

    values.push(input.value);
  }

  for (const label of form.querySelectorAll("label")) {
    if (!label.textContent.endsWith(":")) {
      continue;
    }

    let name = columns[label.textContent.slice(0, -1)].realName;
    if (
      document.getElementById(label.getAttribute("for"))?.tagName === "SELECT"
    ) {
      name += "_id";
    }
    if (name === "position_id" && form.dataset.endpoint === "/seekers") {
      name = "speciality_id";
    }

    names.push(name);
  }

  for (let i = 0; i < names.length; i++) {
    console.log(names[i], "=", values[i]);
  }
  console.log(
    "Are equal:",
    names.length === values.length,
    names.length,
    values.length
  );

  const formData = new FormData();
  names.map((name, i) => formData.append(name, values[i]));
  await put(form.dataset.endpoint, form.dataset.id, formData);
}
