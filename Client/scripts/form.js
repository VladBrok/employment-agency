import columns from "./columns.js";
import endpoints from "./endpoints.js";
import { makeTable } from "./table.js";
import { put, post } from "./api.js";

async function makeForm(
  tableChild,
  buttonText,
  className,
  values = null,
  entityId = null
) {
  let endpoint = tableChild.closest("[data-endpoint]").dataset.endpoint;
  const subEndpoint = endpoint.indexOf("/", 1);
  if (subEndpoint !== -1) {
    endpoint = endpoint.slice(0, subEndpoint);
  }

  const names = Array.from(
    tableChild.closest("[data-endpoint]").querySelectorAll("th")
  )
    .slice(1)
    .map((x) => x.textContent);

  const form = `
    <div class="form-container">
      <img src="images/delete.png" alt="Удалить"
        title="Удалить" class="delete" style="visibility:${
          entityId ? "visible" : "hidden"
        }">
      <form class="crud-form" data-endpoint="${endpoint}" data-id="${entityId}">
        ${(
          await Promise.all(
            names.map(async (name, i) => {
              const id = `${name.replace(/\s+/g, "_")}${i}`;
              const input = await columns[name]?.convertToInput(
                id,
                values?.[i],
                endpoint,
                entityId
              );
              const first = input?.indexOf("photo") !== -1;
              return input === null
                ? ""
                : `<div class="element ${first ? "first" : ""}">
                    <label for="${id}">${name}:</label>
                    ${input}
                  </div>`;
            })
          )
        ).join("")}
        <button type="submit" class="search-button button ${className}">${buttonText}</button>
      </form>
    </div>`;

  const childTables =
    entityId == null
      ? []
      : await Promise.all(
          endpoints[endpoint]?.children.map(
            async (endpoint) => await makeTable({ endpoint, id: entityId })
          ) ?? []
        );

  return `<div class="compound-form main">${form}${childTables.join("")}</div>`;
}

async function sendFormAsPut(e) {
  return await sendForm(
    async (form, data) =>
      await put(form.dataset.endpoint, form.dataset.id, data),
    e
  );
}

async function sendFormAsPost(e) {
  return await sendForm(
    async (form, data) => await post(form.dataset.endpoint, data),
    e
  );
}

async function sendForm(callback, e) {
  const form = document.querySelector(".crud-form");
  let preservePhoto = false;
  const values = [];
  const names = [];

  for (const input of form.querySelectorAll(".input, input:checked")) {
    if (input.checkValidity && !input.checkValidity()) {
      return false;
    }

    if (input.getAttribute("type") === "file") {
      if (
        !input.files[0] &&
        document.querySelector(".photo")?.getAttribute("src") !== "#"
      ) {
        preservePhoto = true;
      } else {
        values.push(input.files[0] ?? "");
      }
    } else {
      values.push(input.value ?? input.getAttribute("value"));
    }
  }

  for (const label of form.querySelectorAll("label")) {
    if (
      !label.textContent.endsWith(":") ||
      (label.textContent.includes("фото") && preservePhoto)
    ) {
      continue;
    }

    let name = columns[label.textContent.slice(0, -1)].realName;
    if (
      document.getElementById(label.getAttribute("for"))?.tagName === "SELECT"
    ) {
      name += "_id";
    }
    if (name === "type_id" && form.dataset.endpoint === "/applications") {
      name = "employment_type_id";
    }

    names.push(name);
  }

  const formData = new FormData();
  values.map((value, i) => formData.append(names[i], value));

  let error = true;
  e.preventDefault();
  await callback(form, formData).then(() => (error = false));
  return !error;
}

export { makeForm, sendFormAsPut, sendFormAsPost };
