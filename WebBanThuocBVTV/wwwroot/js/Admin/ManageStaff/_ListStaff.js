var applyFilters = function () {
	showLoading("");
	var name = document.getElementById("searchInput").value;
	var GioiTinh = document.getElementById("genderSelect").value;
	var NgayTao = document.getElementById("createAtSelect").value;
	var Sort = document.getElementById("sortSelect").value;

	var tableProduct = document.getElementsByClassName("Staff-table")[0];

	$.ajax({
		url: "/Admin/ManageStaff/FilterStaff",
		method: "POST",
		data: { keyword: name, gioiTinh: GioiTinh, ngayTao: NgayTao, sortOption: Sort },
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

	var tableProduct = document.getElementsByClassName("product-table")[0];

	$.ajax({
		url: "/Admin/ManageProduct/SearchProduct",
		method: "POST",
		data: { keyword: "" },
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

var deleteStaff = function (maNv) {
	showComfirm("Bạn có chắc muốn xóa nhân viên này?","Chỉ nhân viên không có dữ liệu mới có thể xóa!", function () {
		showLoading("");
		$.ajax({
			url: "/Admin/ManageStaff/DeleteStaff",
			method: "POST",
			data: { maNd: maNv },
			success: function (res) {
				hideLoading()
				if (res.type == "success") {
					applyFilters(); // Cập nhật lại danh sách nhân viên
				}
				showToast(res.type, res.message);
			},
			error: function (err) {
				hideLoading()
				console.log(err);
			}
		})
    });
}