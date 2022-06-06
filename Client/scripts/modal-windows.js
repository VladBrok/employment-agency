function confirm(message, buttonText) {
  return new Promise((resolve) => {
    addModal(
      `<h1 class="title">Вы уверены?</h1>
      <p class="text">${message}</p>`,
      `<button class="search-button button cancel-button">Отмена</button>
      <button class="search-button button danger-button">${buttonText}</button>`
    );
    document.body.querySelector(".cancel-button").onclick = () =>
      reportResult(resolve, false);
    document.body.querySelector(".danger-button").onclick = () =>
      reportResult(resolve, true);
  });
}

function addModal(content, actions) {
  const modal = `
      <div class="modal">
        <div class="confirm">
          ${content}
          <div class="actions">
            ${actions}
          </div>
        </div>
      </div>`;
  document.body.insertAdjacentHTML("beforeend", modal);
}

function reportResult(callback, result) {
  document.body.querySelector(".modal").remove();
  callback(result);
}

function showInfo(info) {
  addModal(
    `<h1 class="title">${info}</h1>`,
    `<button class="search-button button expanded-button ok">Ок</button>`
  );
  document.body.querySelector(".ok").onclick = () =>
    location.replace("../index.html");
}

export { confirm, showInfo };
