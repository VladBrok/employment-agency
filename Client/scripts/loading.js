const indicator = document.querySelector(".loading");
let isLoading = false;
const DELAY_IN_MILLISECONDS = 300;

export default function loadingDecorator(func) {
  return async function (...args) {
    if (isLoading) {
      await func(...args);
      return;
    }

    isLoading = true;
    const id = setTimeout(
      () => (indicator.style.visibility = "visible"),
      DELAY_IN_MILLISECONDS
    );
    try {
      await func(...args);
    } finally {
      clearTimeout(id);
      indicator.style.visibility = "hidden";
      isLoading = false;
    }
  };
}
