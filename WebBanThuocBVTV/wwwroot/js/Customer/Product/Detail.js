var increaseQuality = function () {
	var soluong_input = $("#quantity-input"); // Lấy phần tử input bằng jQuery
	var soluong = parseInt(soluong_input.val()) || 1; // Lấy giá trị hiện tại, chuyển thành số
	soluong += 1; // Tăng giá trị lên 1
	soluong_input.val(soluong); // Gán giá trị mới cho input
};
var decreaseQuality = function () {
	var soluong_input = $("#quantity-input"); // Lấy phần tử input bằng jQuery
	var soluong = parseInt(soluong_input.val()) || 1; // Lấy giá trị hiện tại, chuyển thành số
	soluong = soluong <= 1 ? 1 : soluong - 1; // Tăng giá trị lên 1
	soluong_input.val(soluong); // Gán giá trị mới cho input
};
var changeQuality = function (input) {
	var soluong = parseInt(input.value) || 1;
	if (soluong < 0)
		input.value = 1;
}
var addGioHang = function () {
	var maSpElement = document.getElementById("maSp");
	var soLuongElement = document.getElementById("quantity-input");
	$.ajax({
		url: "/Customer/Cart/AddProduct",
		type: "POST",
		data: { maSp: maSpElement.value, soLuong: soLuongElement.value },
		success: function (response) {
			if (response.success) {
				showToast("success", response.message);
			} else {
				showToast("warning", response.message);
			}
		},
		error: function (response) {
			showToast('error', response.message);
		}
	});
}