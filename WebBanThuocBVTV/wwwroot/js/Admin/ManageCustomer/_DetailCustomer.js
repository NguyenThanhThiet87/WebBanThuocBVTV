var detailCustomer = function (maNguoiDung) {
	$.ajax({
		url: "/Admin/ManageCustomer/DetailCustomer",
		method: "post",
		data: { maNd: maNguoiDung },
		success: function (res) {
			model_container.innerHTML = res;
		},
		error: function (res) {
			console.log("Lỗi", "error")
		}
	});
}


var enterEditMode = function () {
	var title = document.getElementById("modalTitle")
	title.textContent = "Chỉnh sửa thông tin khách hàng";

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
	var title = document.getElementById("modalTitle")
	title.textContent = "Thông tin khách hàng";

	var lstElementViewMode = document.getElementsByClassName("view-mode");
	var lstElementEditMode = document.getElementsByClassName("edit-mode");
	for (var i = 0; i < lstElementViewMode.length; i++) {
		lstElementViewMode[i].classList.remove("d-none");
	}
	for (var i = 0; i < lstElementEditMode.length; i++) {
		lstElementEditMode[i].classList.add("d-none");
	}
}

var saveEdit = function (maNguoiDung) {
	showLoading("");

	var id = maNguoiDung;
	var name = document.getElementById("editCustomerName").value;
	var gioiTinh = document.getElementById("editCustomerGender").value;
	var ngaySinh = document.getElementById("editCustomerBirthday").value;

	var ngayTao = document.getElementById("customerDateCreated").value;
	var email = document.getElementById("editCustomerEmail").value;
	var soDienThoai = document.getElementById("editCustomerPhone").value;
	var diaChi = document.getElementById("editCustomerAddress").value;

	var dataSend = {
		MaNd: id,
		HoTen: name,
		GioiTinh: gioiTinh,
		NgaySinh: ngaySinh,
		SoDienThoai: soDienThoai,
		DiaChi: diaChi,
		Email: email,
		NgayTao: ngayTao
	}

	$.ajax({
		url: "/Admin/ManageCustomer/UpdateCustomer",
		method: "post",
		data: dataSend,
		success: function (res) {
			model_container.innerHTML = res;
			hideLoading()
			showToast('success', "Cập nhật thành công");
		},
		error: function (res) {
			setAlert("Lỗi", "error")
			hideLoading()
		}
	});
}