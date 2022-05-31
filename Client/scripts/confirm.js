function confirmDelete() {
  return new Promise((resolve) => {
    const modal = `
      <div class="modal">
        <div class="confirm">
          <h1 class="title">Вы уверены?</h1>
          <p class="text">Все зависимые записи также удалятся. Данное действие
            нельзя отменить.</p>
          <div class="actions">
            <button class="search-button button cancel-button">Отмена</button>
            <button class="search-button button delete-button">Удалить</button>
          </div>
        </div>
      </div>`;
    document.body.insertAdjacentHTML("beforeend", modal);
    document.body.querySelector(".cancel-button").onclick = () =>
      confirm(resolve, false);
    document.body.querySelector(".delete-button").onclick = () =>
      confirm(resolve, true);
  });
}

function confirm(callback, result) {
  document.body.querySelector(".modal").remove();
  callback(result);
}

export default confirmDelete;
