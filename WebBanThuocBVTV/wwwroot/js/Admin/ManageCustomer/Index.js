document.addEventListener("DOMContentLoaded", () => {
	applyFilters();
})

var detailCustomer_modal = document.getElementById('detail_customerModal');
detailCustomer_modal.addEventListener('hide.bs.modal', function (event) {
	applyFilters();
});

var model_container = document.getElementById("modal-container");
