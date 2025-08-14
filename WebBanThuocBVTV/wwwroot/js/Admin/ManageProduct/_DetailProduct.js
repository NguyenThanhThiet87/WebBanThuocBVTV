//detailProduct
var detailProductModal = function (maSanPham) {
	$.ajax({
		url: "/Admin/ManageProduct/DetailProduct",
		method: "POST",
		data: { maSp: maSanPham },
		success: function (res) {
			model_container.innerHTML = res
		},
		error: function (err) {
			console.log(err)
		}
	})
}
var enterEditMode = function () {
	var lstElementViewMode = document.getElementsByClassName("view-mode");
	var lstElementEditMode = document.getElementsByClassName("edit-mode");
	for (var i = 0; i < lstElementViewMode.length; i++) {
		lstElementViewMode[i].classList.add("d-none");
	}
	for (var i = 0; i < lstElementEditMode.length; i++) {
		lstElementEditMode[i].classList.remove("d-none");
	}
}

var cancelEditMode = function () {
	var lstElementViewMode = document.getElementsByClassName("view-mode");
	var lstElementEditMode = document.getElementsByClassName("edit-mode");
	for (var i = 0; i < lstElementViewMode.length; i++) {
		lstElementViewMode[i].classList.remove("d-none");
	}
	for (var i = 0; i < lstElementEditMode.length; i++) {
		lstElementEditMode[i].classList.add("d-none");
	}
}

var saveEdit = function (maSanPham) {
	showLoading("");

	var id = maSanPham;
	var name = document.getElementById("editProductName").value;
	var ingre = document.getElementById("editProductIngredient").value;
	var usage = document.getElementById("editProductUse").value;
	var guide = document.getElementById("editProductGuide").value;
	var price = document.getElementById("editProductPrice").value;
	var quantity = document.getElementById("editProductQuality").value;
	var expiry = document.getElementById("editProductExpiry").value;
	var category = document.getElementById("editProductCategory").value;
	var provider = document.getElementById("editProductProvider").value;

	var dataSend = {
		MaSanPham: id,
		TenSanPham: name,
		ThanhPhan: ingre,
		CongDung: usage,
		HuongDanSd: guide,
		Gia: price,
		SoLuong: quantity,
		MaNhomSp: category,
		MaNhaSx: provider,
		HanSd: expiry
	}

	$.ajax({
		url: "/Admin/ManageProduct/UpdateProduct",
		method: "post",
		data: dataSend,
		success: function (res) {
			if (res.type == "success")
				detailProductModal(id);
			hideLoading();
			showToast(res.type, res.message);
		},
		error: function (err) {
			console.log("Lỗi", err);
			hideLoading();
		}
	});
}
