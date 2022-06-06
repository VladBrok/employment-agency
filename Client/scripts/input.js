document.addEventListener("input", (e) => {
  e.target.setCustomValidity("");
});

document.addEventListener("click", (e) => {
  if (!e.target.classList.contains("button") || !e.target.closest("form")) {
    return;
  }

  [...document.querySelectorAll("[data-error]")]
    .filter((i) => i.validity.patternMismatch)
    .forEach((i) => {
      i.setCustomValidity(i.dataset.error);
      i.reportValidity();
    });
});

function makeFileInput(id, value, accept, required = false) {
  const input = makeInput(id, value, "file", required);
  input.setAttribute("accept", accept);
  return input.outerHTML;
}

function makeDateInput(id, value, min, max, required = false) {
  return makeMinMaxInput(id, formatDate(value), "date", min, max, required);
}

function makeNumberInput(id, value, min, max, required = false) {
  const comma = value?.indexOf(",") ?? -1;
  return makeMinMaxInput(
    id,
    comma === -1 ? value : value.slice(0, comma),
    "number",
    min,
    max,
    required
  );
}

function makeMinMaxInput(id, value, type, min, max, required = false) {
  const input = makeInput(id, value, type, required);
  input.setAttribute("min", min);
  input.setAttribute("max", max);
  return input.outerHTML;
}

function makeTextInput(
  id,
  value,
  minlength = null,
  maxlength = null,
  required = false,
  pattern = null,
  errorMessage = null
) {
  const input = makeInput(id, value, "text", required);
  if (minlength) {
    input.setAttribute("minlength", minlength);
  }
  if (maxlength) {
    input.setAttribute("maxlength", maxlength);
  }
  if (pattern) {
    input.setAttribute("pattern", pattern);
  }
  if (errorMessage) {
    input.setAttribute("data-error", errorMessage);
  }
  return input.outerHTML;
}

function makeEmailInput(id, value, required = false) {
  return makeInput(id, value, "email", required).outerHTML;
}

function makeInput(id, value, type, required = false) {
  const input = document.createElement("input");
  input.setAttribute("id", id);
  input.setAttribute("type", type);
  if (value) {
    input.setAttribute("value", value);
  }
  if (required) {
    input.setAttribute("required", "");
  }
  input.className = "input";
  return input;
}

function formatDate(date) {
  if (!date) {
    return;
  }
  const dayMonthYear = date.split(".");
  return dayMonthYear.reverse().join("-");
}

const PATTERNS = {
  LETTER_ONLY:
    "[A-Za-zАаБбВвГгДдЕеЁеЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчЩщЪъЫыЬьЭэЮюЯя]+",
};
export {
  makeFileInput,
  makeDateInput,
  makeNumberInput,
  makeTextInput,
  makeEmailInput,
  PATTERNS,
};
