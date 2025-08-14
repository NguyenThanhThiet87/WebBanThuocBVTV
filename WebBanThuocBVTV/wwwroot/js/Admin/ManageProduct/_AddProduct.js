var addProductModal = function () {
	$.ajax({
		url: "/Admin/ManageProduct/AddProductPartialView",
		method: "get",
		success: function (res) {
			model_container.innerHTML = res
		},
		error: function (err) {
			console.log(err)
		}
	})
}
var reviewImage = function (input) {
	var file = input.files[0];
	var previewElement = document.getElementById("imageProduct");
	previewElement.src = URL.createObjectURL(file);
}
var addProduct = function () {

	showLoading("");
	var id = document.getElementById("productId").value;
	var name = document.getElementById("productName").value;
	var category = document.getElementById("category").value;
	var price = document.getElementById("price").value;
	var quanlity = document.getElementById("quantity").value;
	var provider = document.getElementById("provider").value;
	var expiry = document.getElementById("expiryDate").value;
	var composition = document.getElementById("composition").value;
	var usage = document.getElementById("usage").value;
	var instructions = document.getElementById("instructions").value;
	var imgFile = document.getElementById("imageInput").files[0];

	const formData = new FormData();

	formData.append('sp.MaSanPham', id);
	formData.append('sp.TenSanPham', name);
	formData.append('sp.MaNhomSp', category);
	formData.append('sp.MaNhaSx', provider);
	formData.append('sp.Gia', price);
	formData.append('sp.SoLuong', quanlity);
	formData.append('sp.HanSd', expiry);
	formData.append('sp.ThanhPhan', composition);
	formData.append('sp.CongDung', usage);
	formData.append('sp.HuongDanSd', instructions);

	formData.append('imgProduct', imgFile);

	$.ajax({
		url: "/Admin/ManageProduct/AddProduct",
		method: "post",
		data: formData,
		contentType: false,
		processData: false,
		success: function (res) {
			hideLoading()
			showToast(res.type, res.message);
			addProductModal()
		},
		error: function (err) {
			hideLoading()
			console.log(err)
		}
	})
}