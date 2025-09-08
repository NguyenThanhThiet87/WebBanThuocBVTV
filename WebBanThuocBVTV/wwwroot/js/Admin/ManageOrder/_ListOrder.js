var applyFilters = function (status) {
	if (status != "init")
		showLoading("");
	var name = document.getElementById("searchInput").value;
	var TrangThai = document.getElementById("stateSelect").value;
	var Sort = document.getElementById("sortSelect").value;

	var tableProduct = document.getElementsByClassName("order-table")[0];

	$.ajax({
		url: "/Admin/ManageOrder/FilterOrder",
		method: "POST",
		data: { id: name, state: TrangThai, sortOption: Sort },
		success: function (res) {
			hideLoading()
			tableProduct.innerHTML = res;
		},
		error: function (err) {
			hideLoading()
			console.log(err);
		}
	})
}

var resetFilters = function () {
	showLoading("");
	var TrangThai = document.getElementById("stateSelect");
	TrangThai.value = "DXL";

	searchOrder();
}