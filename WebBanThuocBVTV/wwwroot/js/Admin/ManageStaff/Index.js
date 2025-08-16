document.addEventListener("DOMContentLoaded", () => {
	applyFilters();
})

var detailStaff_modal = document.getElementById('detail_StaffModal');
detailStaff_modal.addEventListener('hide.bs.modal', function (event) {
	applyFilters();
});

var model_container = document.getElementById("modal-container");
