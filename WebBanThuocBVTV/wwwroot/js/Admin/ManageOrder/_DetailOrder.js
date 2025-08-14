var detailOrder = function (maDonHang) {
	$.ajax({
		url: "/Admin/ManageOrder/DetailOrder",
		method: "post",
		data: { maDh: maDonHang },
		success: function (res) {
			model_container.innerHTML = res;
		},
		error: function (res) {
			console.log(res);
		}
	});
}
var sendOrder = function (maDonHang) {
	showLoading("");
	$.ajax({
		url: "/Admin/ManageOrder/SendOrder",
		method: "post",
		data: { maDh: maDonHang },
		success: function (res) {
			if (res.type = "success") {
				detailOrder(maDonHang);
			}
			hideLoading();
			showToast(res.type, res.message);
		},
		error: function (res) {
			hideLoading();
			console.log(res);
		}
	});
}
var transferredOrder = function (maDonHang) {
	showLoading("");
	$.ajax({
		url: "/Admin/ManageOrder/TransferredOrder",
		method: "post",
		data: { maDh: maDonHang },
		success: function (res) {
			if (res.type = "success") {
				detailOrder(maDonHang);
			}
			hideLoading();
			showToast(res.type, res.message);
		},
		error: function (res) {
			hideLoading();
			console.log(res);
		}
	});
}