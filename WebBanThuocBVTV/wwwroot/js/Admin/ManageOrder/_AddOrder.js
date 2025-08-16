var addOrderModal = function () {
    $.ajax({
        url: "/Admin/ManageOrder/AddOrderPartialView",
        method: "POST",
        success: function (res) {
            model_container.innerHTML = res;
        },
        error: function (res) {
            console.log(res);
        }
    });
}
var toggleTabAddOrder = function (element, target) {
    var tab = document.getElementsByClassName("tab-button");
    for (var item of tab) {
        item.classList.remove("active");
    }

    var guest_tab = document.getElementById("guest-tab");
    var customer_tab = document.getElementById("customer-tab");
    if (target == "guest") {
        guest_tab.classList.add("active");
        customer_tab.classList.remove("active");
    } else {
        customer_tab.classList.add("active");
        guest_tab.classList.remove("active");
    }
    element.classList.add("active");
}
var addProductOrder = function () {
    var productListContainer = document.getElementById("product-list");
    productListContainer.insertAdjacentHTML('beforeend', `<div class="product-row">
					<div class="form-group">
						<label>Sản phẩm</label>
						<input class="maSp" type="text" placeholder="Mã sản phẩm" onchange="getSp(this)">
					</div>

					<div class="form-group">
						<label>Tên sản phẩm</label>
						<input class="nameProduct" type="text" readonly>
					</div>

					<div class="form-group">
						<label>Số lượng</label>
						<input min="1" class="quantityProduct" type="number" onchange="SumPrice(this)">
					</div>

					<div class="form-group">
						<label>Đơn giá</label>
						<input class="priceProduct" type="text" placeholder="vnđ" readonly>
					</div>

					<div class="form-group">
						<label>Thành tiền</label>
						<input class="totalProduct" type="text" placeholder="vnđ" readonly>
					</div>

					<button class="remove-btn" onclick="deleteProductOrder(this)">
						<i class="fa-solid fa-trash" style="color: #ffffff;"></i>
					</button>
				</div>`)
}

var deleteProductOrder = function (btn) {
    var listProduct = btn.parentNode.parentNode;
    if (listProduct.children.length > 1) {
        btn.parentNode.remove();
    } else {
        showToast("warning", "Đơn hàng phải có ít nhất một sản phẩm");
    }
}

var searchCustomer = function () {
    showLoading("");
    var maNguoiDung = document.getElementById("customer-code").value;

    var maNd = document.getElementById("customer-id");
    var name = document.getElementById("customer-name");
    var phone = document.getElementById("customer-phone");
    var email = document.getElementById("customer-email");
    var gender = document.getElementById("customer-gender");
    var address = document.getElementById("customer-address");

    $.ajax({
        url: "/Admin/ManageOrder/GetInfoCustomer",
        method: "POST",
        data: { maNd: maNguoiDung },
        success: function (res) {
            if (res.success) {
                maNd.value = res.data.maNd;
                name.value = res.data.hoTen;
                phone.value = res.data.soDienThoai;
                email.value = res.data.email;
                gender.value = res.data.displayGioiTinh;
                address.value = res.data.diaChi;
                hideLoading();
                console.log(res)
            } else {
                name.value = "";
                phone.value = "";
                email.value = "";
                gender.value = "";
                address.value = "";
                showToast("warning", "Người dùng không tồn tại");
            }
        },
        error: function (res) {
            console.log(res);
        }
    });
}
var getSp = function (input) {
    var maSanPham = input.value;
    var parent = input.parentNode.parentNode;
    var nameProduct = parent.querySelector('.nameProduct');
    var quantityProduct = parent.querySelector('.quantityProduct');
    var priceProduct = parent.querySelector('.priceProduct');
    var totalProduct = parent.querySelector('.totalProduct');

    var quantityStore = parent.querySelector('.quantityStore');

    $.ajax({
        url: "/Admin/ManageOrder/GetInfoProduct",
        data: { maSp: maSanPham },
        method: "POST",
        success: function (res) {
            if (res.success) {
                nameProduct.value = res.data.tenSanPham;
                quantityProduct.value = 1;
                priceProduct.value = new Intl.NumberFormat('vi-VN').format(res.data.gia) + "VNĐ";
                quantityStore.value = res.data.soLuong;
                SumPrice(quantityProduct);
            } else {
                nameProduct.value = "";
                quantityProduct.value = 1;
                priceProduct.value = "";
                totalProduct.value = "";
                showToast("warning", "Không tìm thấy sản phẩm")
            }
        },
        error: function (res) {
            console.log(res);
        }
    });
}

var SumPrice = function (input) {
    var quantityStore = input.nextElementSibling.value;
    
    var quantity = input.value;
    if (quantity < 1) {
        input.value = 1;
        quantity = 1;
    }
    if (quantity > quantityStore) {
        input.value = quantityStore;
        showToast("warning", "Vượt quá số lượng");
    }
    var parent = input.parentNode.parentNode;

    var price = parseInt(parent.querySelector('.priceProduct').value.replace(/\D/g, ''));
    var sumPrice = parent.querySelector('.totalProduct');
    var total = new Intl.NumberFormat('vi-VN').format(quantity * price);
    sumPrice.value = total + "VNĐ";
    SumTotalPrice();
}

var SumTotalPrice = function () {
    var sumPrice = 0
    var totalElement = document.getElementById("guest-total");
    var priceProducts = document.getElementsByClassName("totalProduct");
    for (var input of priceProducts) {
        sumPrice += parseInt(input.value.replace(/\D/g, ''));
    }
    totalElement.textContent = new Intl.NumberFormat('vi-VN').format(sumPrice) + "VNĐ";
}
var addOrder = function () {
    var productListContainer = document.getElementById("product-list");
    var guest_tab = document.getElementById("guest-tab");
    var customer_tab = document.getElementById("customer-tab");
    if (guest_tab.classList.contains("active")) {
        if (!guest_tab.checkValidity()) {
            guest_tab.reportValidity();
            return;
        }
        showLoading("");
        var name = document.getElementById("guest-name");
        var phone = document.getElementById("guest-phone");
        var email = document.getElementById("guest-email");
        var gender = document.getElementById("guest-gender");
        var address = document.getElementById("guest-address");

        var nguoiDung = {
            HoTen: name.value,
            SoDienThoai: phone.value,
            Email: email?.value,
            GioiTinh: gender.value,
            DiaChi: address.value
        }

        var PhuongThucTt = "NH";
        var total = parseInt(document.getElementById("guest-total").textContent.replace(/\D/g, ''));
        var note = document.getElementById("note-guest");

        var maSpArray = []
        var lstProductRow = document.getElementsByClassName("product-row");

        for (var sp of lstProductRow) {
            var maSp = sp.querySelector(".maSp")?.value;
            var tenSp = sp.querySelector(".nameProduct")?.value;
            if (tenSp != "") {
                var quantity = sp.querySelector(".quantityProduct").value;
                var totalProduct = parseInt(sp.querySelector(".totalProduct").value.replace(/\D/g, ''));

                maSpArray.push({ MaSanPham: maSp, SoLuongDatMua: quantity, TongTien: totalProduct });
            }
        }
        if (maSpArray.length < 1) {
            hideLoading();
            showToast("warning", "Đơn hàng phải có ít nhất 1 sản phẩm");
            return;
        }
        var donHang = {
            TongTien: total,
            GhiChu: note.value,
            MaPhuongThucTt: PhuongThucTt,
            DonHangSanPhams: maSpArray
        }

        $.ajax({
            url: "/Admin/ManageOrder/AddOrderGuest",
            method: "POST",
            data: { dh: donHang, nd: nguoiDung },
            success: function (res) {
                name.value = "";
                phone.value = "";
                email.value = "";
                address.value = "";
                document.getElementById("guest-total").textContent = "VNĐ";
                note.value = "";

                productListContainer.innerHTML = "";
                addProductOrder();
                hideLoading();
                showToast(res.type, res.message);
            },
            error: function (res) {
                console.log(res);
            }
        });
    } else if (customer_tab.classList.contains("active")) {
        showLoading("");
        var maNd = document.getElementById("customer-id");
        var tenNd = document.getElementById("customer-name").value;
        if (tenNd == "") {
            hideLoading();
            showToast("warning", "Tên người dùng không hợp lệ");
            return;
        }
        var PhuongThucTt = "NH";
        var total = parseInt(document.getElementById("guest-total").textContent.replace(/\D/g, ''));
        var note = document.getElementById("note-guest");

        var maSpArray = []
        var lstProductRow = document.getElementsByClassName("product-row");

        for (var sp of lstProductRow) {
            var maSp = sp.querySelector(".maSp")?.value;
            var tenSp = sp.querySelector(".nameProduct")?.value;
            if (tenSp != "") {
                var quantity = sp.querySelector(".quantityProduct").value;
                var totalProduct = parseInt(sp.querySelector(".totalProduct").value.replace(/\D/g, ''));

                maSpArray.push({ MaSanPham: maSp, SoLuongDatMua: quantity, TongTien: totalProduct });
            }
        }
        if (maSpArray.length < 1) {
            hideLoading();
            showToast("warning", "Đơn hàng phải có ít nhất 1 sản phẩm");
            return;
        }
        var donHang = {
            MaNd: maNd.value,
            TongTien: total,
            GhiChu: note.value,
            MaPhuongThucTt: PhuongThucTt,
            DonHangSanPhams: maSpArray
        }

        $.ajax({
            url: "/Admin/ManageOrder/AddOrder",
            method: "POST",
            data: { dh: donHang },
            success: function (res) {
                document.getElementById("guest-total").textContent = "VNĐ";
                note.value = "";
                maNd.value = "";
                var maNdElement = document.getElementById("customer-id");
                var nameElement = document.getElementById("customer-name");
                var phoneElement = document.getElementById("customer-phone");
                var emailElement = document.getElementById("customer-email");
                var genderElement = document.getElementById("customer-gender");
                var addressElement = document.getElementById("customer-address");
                maNdElement.value = "";
                nameElement.value = "";
                phoneElement.value = "";
                emailElement.value = "";
                genderElement.value = "";
                addressElement.value = "";

                productListContainer.innerHTML = "";
                addProductOrder();
                hideLoading();
                showToast(res.type, res.message);
            },
            error: function (res) {
                console.log(res);
            }
        });
    }
    
}