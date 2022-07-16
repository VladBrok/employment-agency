const indicator = document.querySelector(".loading");
let isLoading = false;
const DELAY_IN_MILLISECONDS = 300;

export default function loadingDecorator(func) {
  return async function (e) {
    if (isLoading) {
      await func(e);
      return;
    }

    isLoading = true;
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
      indicator.style.visibility = "hidden";
      e.target.classList.remove("disabled-because-loading");
      e.target.removeAttribute("tabindex");
      isLoading = false;
    }
  };
}
