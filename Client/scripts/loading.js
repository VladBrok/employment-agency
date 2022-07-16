const indicator = document.querySelector(".loading");
let loadingCount = 0;
const DELAY_IN_MILLISECONDS = 300;

export default function loadingDecorator(func) {
  return async function (e) {
    if (e.target.classList.contains("disabled-because-loading")) {
      return;
    }

    loadingCount++;
    e.target.classList.add("disabled-because-loading");
    e.target.setAttribute("tabindex", -1);
    const id = setTimeout(
      () => (indicator.style.visibility = "visible"),
      DELAY_IN_MILLISECONDS
    );

    try {
      await func(e);
    } finally {
      clearTimeout(id);
      e.target.classList.remove("disabled-because-loading");
      e.target.removeAttribute("tabindex");
      loadingCount--;
      if (loadingCount <= 0) {
        indicator.style.visibility = "hidden";
      }
    }
  };
}
