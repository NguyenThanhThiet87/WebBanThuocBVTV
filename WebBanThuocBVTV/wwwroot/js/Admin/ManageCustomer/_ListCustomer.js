var applyFilters = function () {
	showLoading("");
	var name = document.getElementById("searchInput").value;
	var Category = document.getElementById("categorySelect").value;
	var GioiTinh = document.getElementById("genderSelect").value;
	var NgayTao = document.getElementById("createAtSelect").value;
	var Sort = document.getElementById("sortSelect").value;

	var tableProduct = document.getElementsByClassName("customer-table")[0];

	$.ajax({
		url: "/Admin/ManageCustomer/FilterCustomer",
		method: "POST",
		data: { keyword: name, loaiKh: Category, gioiTinh: GioiTinh, ngayTao: NgayTao, sortOption: Sort },
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