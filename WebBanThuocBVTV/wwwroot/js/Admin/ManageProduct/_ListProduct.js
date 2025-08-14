var pageList = function (a, current_page, count_page) {
	var pageNumber = a.text;

	if (pageNumber == "Previous") {
		pageNumber = current_page - 1;
		pageNumber = pageNumber > 1 ? pageNumber : 1;
	} else if (pageNumber == "Next") {
		pageNumber = current_page + 1;
		pageNumber = pageNumber > count_page ? current_page : pageNumber;
	}
	console.log(pageNumber)

	var tableProduct = document.getElementsByClassName("product-table")[0];

	$.ajax({
		url: "/Admin/ManageProduct/SearchProduct",
		method: "POST",
		data: { keyword: "", page: pageNumber },
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



var applyFilters = function () {
	showLoading("");
	var trangThaiSp = document.getElementById("activeGroupSelect").value;
	var nhomSp = document.getElementById("productGroupSelect").value;
	var nhaSx = document.getElementById("manufacturerSelect").value;
	var name = document.getElementById("searchInput").value;
	var sort = document.getElementById("sortSelect").value;

	var priceRadio = document.getElementsByName("priceRange");
	var price = "";
	for (var radio of priceRadio) {
		if (radio.checked) {
			price = radio.value
			break;
		}
	}
	var quantityRadio = document.getElementsByName("stockStatus");
	var quantity = "";
	for (var radio of quantityRadio) {
		if (radio.checked) {
			quantity = radio.value
		}
	}

	var tableProduct = document.getElementsByClassName("product-table")[0];

	$.ajax({
		url: "/Admin/ManageProduct/FilterProduct",
		method: "POST",
		data: { keyword: name, isActive: trangThaiSp, maNhomSp: nhomSp, maNhaSx: nhaSx, sortOption: sort, priceArrange: price, quantityOption: quantity },
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
	var name = document.getElementById("searchInput").value;
	var sort = document.getElementById("sortSelect").value;

	var nhomSp = document.getElementById("productGroupSelect");
	var nhaSx = document.getElementById("manufacturerSelect");
	var priceRadio = document.getElementsByName("priceRange");
	nhomSp.value = "";
	nhaSx.value = "";
	for (var radio of priceRadio) {
		radio.checked = false;
	}
	var quantityCheckBox = document.getElementsByName("stockStatus");
	for (var checkbox of quantityCheckBox) {

		checkbox.checked = false;
	}
	$.ajax({
		url: "/Admin/ManageProduct/FilterProduct",
		method: "POST",
		data: { keyword: name, sortOption: sort },
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
