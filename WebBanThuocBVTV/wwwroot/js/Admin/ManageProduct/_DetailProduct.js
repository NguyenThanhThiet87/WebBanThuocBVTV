//detailProduct
var easyMDE;
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
	easyMDE = new EasyMDE({
		element: document.getElementById('editProductGuide')
	});
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
	easyMDE.toTextArea();
	easyMDE = null;
}

var saveEdit = function (maSanPham) {
	showLoading("");

	var id = maSanPham;
	var name = document.getElementById("editProductName").value;
	var ingre = document.getElementById("editProductIngredient").value;
	var usage = document.getElementById("editProductUse").value;
	var guide = easyMDE.value();
	var price = document.getElementById("editProductPrice").value;
	var quantity = document.getElementById("editProductQuality").value;
	var expiry = document.getElementById("editProductExpiry").value;
	var category = document.getElementById("editProductCategory").value;
	var provider = document.getElementById("editProductProvider").value;
	var isActive = document.getElementById("isActive").value;

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
		HanSd: expiry,
		isActive: isActive
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

var deleteProduct = function (maSanPham) {
	showComfirm("Bạn có chắc muốn xóa sản phẩm?", "Hãy cân nhắc trước khi xóa!", () => {
		showLoading("");
		$.ajax({
			url: "/Admin/ManageProduct/DeleteProduct",
			method: "post",
			data: { maSp: maSanPham },
			success: function (res) {
				if (res.type == "success") {
					modal.hide();
					applyFilters();
				}
				hideLoading();
				showToast(res.type, res.message);
			},
			error: function (err) {
				console.log("Lỗi", err);
				hideLoading();
			}
		});
	})
}
