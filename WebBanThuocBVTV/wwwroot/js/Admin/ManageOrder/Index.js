document.addEventListener("DOMContentLoaded", () => {
	applyFilters();
})

var detailOrder_modal = document.getElementById('detail_orderModal');
detailOrder_modal.addEventListener('hide.bs.modal', function (event) {
	applyFilters("init");
});

var model_container = document.getElementById("modal-container");


var deleteOrder = function (maDonHang) {
	showLoading("");
	$.ajax({
		url: "/Admin/ManageOrder/DeleteOrder",
		method: "post",
		data: { maDh: maDonHang },
		success: function (res) {
			if (res.type = "success") {
				applyFilters("init");
			}
			console.log(res)
			hideLoading();
			showToast(res.type, res.message);
		},
		error: function (res) {
			hideLoading();
			console.log(res);
		}
	});
}