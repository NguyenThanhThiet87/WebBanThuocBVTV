var detailStaff = function (maNguoiDung) {
	$.ajax({
		url: "/Admin/ManageStaff/DetailStaff",
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
	title.textContent = "Chỉnh sửa thông tin nhân viên";

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
	title.textContent = "Thông tin nhân viên";

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
	var name = document.getElementById("editStaffName").value;
	var gioiTinh = document.getElementById("editStaffGender").value;
	var ngaySinh = document.getElementById("editStaffBirthday").value;

	var ngayTao = document.getElementById("StaffDateCreated").value;
	var email = document.getElementById("editStaffEmail").value;
	var soDienThoai = document.getElementById("editStaffPhone").value;
	var diaChi = document.getElementById("editStaffAddress").value;

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
		url: "/Admin/ManageStaff/UpdateStaff",
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