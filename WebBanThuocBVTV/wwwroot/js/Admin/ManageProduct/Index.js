var model_container = document.getElementById("modal-container");

document.addEventListener("DOMContentLoaded", () => {
	applyFilters();
})

var discontinueProduct = function (maSanPham) {
	showLoading("");
	$.ajax({
		url: "/Admin/ManageProduct/DiscontinueProduct",
		method: "POST",
		data: { maSp: maSanPham },
		success: function (res) {
			if (res.type == "success") {
				applyFilters();
			}
			showToast(res.type, res.message);
			hideLoading();
		},
		error: function (err) {
			console.log(err)
			hideLoading();
		}
	})
}
var sellProduct = function (maSanPham) {
	showLoading("");
	$.ajax({
		url: "/Admin/ManageProduct/SellProduct",
		method: "POST",
		data: { maSp: maSanPham },
		success: function (res) {
			if (res.type == "success") {
				applyFilters();
			}
			showToast(res.type, res.message);
			hideLoading();
		},
		error: function (err) {
			console.log(err)
			hideLoading();
		}
	})
}