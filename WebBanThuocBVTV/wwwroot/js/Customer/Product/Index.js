document.addEventListener("DOMContentLoaded", () => {
	applyFilter();
})

function selectGroup(item, maNhomSp) {
	var lstNhomSp = document.getElementsByClassName("categoryProduct");
	for (var nhomSp of lstNhomSp) {
		nhomSp.classList.remove("active");
	}
	item.classList.add("active");

	document.getElementById('maNhomSp').value = maNhomSp;
	applyFilter();
}

$(document).ready(function () {
	$('.banner-product').slick({
		infinite: true,   // Vòng lặp vô tận
		speed: 500,       // Tốc độ chuyển động
		slidesToShow: 4,  // Số slide hiển thị cùng lúc
		slidesToScroll: 1, // Số slide di chuyển mỗi lần
		autoplay: true,
		autoplaySpeed: 2500,
		arrows: true,
		vertical: true,
	})
});

var applyFilter = function () {
	showLoading("");
	var nhomSp = document.getElementById('maNhomSp').value;
	var nhaSx = document.getElementById("manufactorFilter").value;
	var price = document.getElementById("priceFilter").value;
	// var rate = document.getElementById("ratingFilter").value;

	var tableProduct = document.getElementsByClassName("listProduct")[0];

	console.log(nhomSp)
	$.ajax({
		url: "/Customer/Product/FilterProduct",
		method: "POST",
		data: { maNhomSp: nhomSp, maNhaSx: nhaSx, sortPrice: price },
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
var pageList = function (a, current_page, count_page) {
	var pageNumber = a.text;

	if (pageNumber == "Previous") {
		pageNumber = current_page - 1;
		pageNumber = pageNumber > 1 ? pageNumber : 1;
	} else if (pageNumber == "Next") {
		pageNumber = current_page + 1;
		pageNumber = pageNumber > count_page ? current_page : pageNumber;
	}

	var nhomSp = document.getElementById('maNhomSp').value;
	var nhaSx = document.getElementById("manufactorFilter").value;
	var price = document.getElementById("priceFilter").value;

	var tableProduct = document.getElementsByClassName("listProduct")[0];

	$.ajax({
		url: "/Customer/Product/FilterProduct",
		method: "POST",
		data: { maNhomSp: nhomSp, maNhaSx: nhaSx, sortPrice: price, page: pageNumber },
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