function confirm(message, buttonText) {
  return new Promise((resolve) => {
    addModal(
      `<h1 class="title">Вы уверены?</h1>
      <p class="text">${message}</p>`,
      `<button class="search-button button cancel-button">Отмена</button>
      <button class="search-button button danger-button">${buttonText}</button>`
    );
    document.body.querySelector(".cancel-button").onclick = () =>
      hideModal(resolve, false);
    document.body.querySelector(".danger-button").onclick = () =>
      hideModal(resolve, true);
  });
}

function addModal(content, actions) {
  const modal = `
      <div class="modal" tabindex="-1">
        <div class="confirm">
          ${content}
          <div class="actions">
            ${actions}
          </div>
        </div>
      </div>`;
  document.body.insertAdjacentHTML("beforeend", modal);
  document.querySelector(".modal").focus();
}

function hideModal(callback, result = null) {
  document.body.querySelector(".modal").remove();
  callback(result);
}

function showInfo(handleOk, info) {
  addModal(
    `<h1 class="title">${info}</h1>`,
    `<button class="search-button button expanded-button ok">Ок</button>`
  );
  document.body.querySelector(".ok").onclick = () => hideModal(handleOk);
}

export { confirm, showInfo };
