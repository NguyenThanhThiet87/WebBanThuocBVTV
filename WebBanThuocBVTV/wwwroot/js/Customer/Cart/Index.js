//Tải dữ liệu giỏ hàng khi load web
window.addEventListener('load', async () => {
    var cart_container = document.getElementById("cart-container");
    if (cart_container != null) {
        try {
            var cart_container = document.getElementById("cart-container");
            $.ajax({
                url: "/Customer/Cart/Index",
                type: "GET",
                success: function (response) {
                    cart_container.innerHTML = ''; // Xóa các kết quả cũ
                    cart_container.innerHTML = response; // Hiển thị ô kết quả
                },
                error: function (xhr, status, error) {
                    console.error("Lỗi :", error);
                }
            })
        } catch (error) {
            console.error('Lỗi khi gọi API:', error);
        }
    }
})
//tải dữ liệu giỏ hàng khi muốn hiển thị
document.addEventListener('show.bs.modal', (e) => {
    if (e.target.id === 'cartModalToggle') {
        var cart_container = document.getElementById("cart-container");
        $.ajax({
            url: "/Customer/Cart/Index",
            type: "GET",
            success: function (response) {
                cart_container.innerHTML = ''; // Xóa các kết quả cũ
                cart_container.innerHTML = response; // Hiển thị ô kết quả
            },
            error: function (xhr, status, error) {
                console.error("Lỗi :", error);
            }
        })
    }
})
var updateSoLuongOnServer = async function (maSpCart, soluong) {
    $.ajax({
        url: "/Customer/Cart/UpdateSoluongProduct",
        type: "POST",
        data: { maSp: maSpCart.value, soLuong: soluong },
        success: function (response) {
            console.log(response.message);
        },
        error: function (response) {
            console.log(response.message);
        }
    })
}
var increaseQualityCart = function (btn) {
    var soluong_input = btn.previousElementSibling;
    var soluong = parseInt(soluong_input.value) || 1; //Lấy giá trị hiện tại, chuyển thành số
    var soLuongTonElement = btn.parentElement.querySelector('.soLuongSpTon')
    if (soluong >= soLuongTonElement.value) {
        showToast("warning", "Vượt quá số lượng tồn kho");
    } else {
        soluong = soluong + 1; //Tăng giá trị lên 1
        soluong_input.value = soluong; //Gán giá trị mới cho input
        var maSpCart = soLuongTonElement.previousElementSibling;
        updateSoLuongOnServer(maSpCart, soluong);
        //cập nhật giá mới
        var gia = btn.parentElement.parentElement.previousElementSibling;
        var sum = btn.parentElement.parentElement.nextElementSibling;
        sum.innerHTML = (soluong * Number(gia.innerHTML.replace("VNĐ", "").replaceAll(".", ""))).toLocaleString("vi-VN") + " VNĐ";
        update_SumPrice();
    }
};

var decreaseQualityCart = function (btn) {
    var soluong_input = btn.nextElementSibling; // Lấy phần tử input bằng jQuery
    var soluong = parseInt(soluong_input.value) || 1; // Lấy giá trị hiện tại, chuyển thành số
    var soLuongTonElement = btn.parentElement.querySelector('.soLuongSpTon')

    soluong = soluong <= 1 ? 1 : soluong - 1; // giảm giá trị lên 1
    if (soluong > soLuongTonElement.value) {
        showToast("warning", "Vượt quá số lượng tồn kho");
        soluong = soLuongTonElement.value;
        soluong_input.value = soLuongTonElement.value;
    } else {
        soluong_input.value = soluong; // Gán giá trị mới cho input
        var maSpCart = btn.parentElement.querySelector('.soLuongSpTon').previousElementSibling;
        updateSoLuongOnServer(maSpCart, soluong);
        //cập nhật giá mới
        var gia = btn.parentElement.parentElement.previousElementSibling;
        var sum = btn.parentElement.parentElement.nextElementSibling;
        sum.innerHTML = (soluong * Number(gia.innerHTML.replace("VNĐ", "").replaceAll(".", ""))).toLocaleString("vi-VN") + " VNĐ";
        update_SumPrice();
    }
};

var update_SumPrice = function () {
    var sum = 0;
    var lstSp = document.getElementsByClassName("sp");
    console.log(lstSp);
    for (var sp of lstSp) {
        if (sp.querySelector(".checkbox").checked) {
            sum += Number(sp.querySelector("#total-0").innerHTML.replace("VNĐ", "").replaceAll(".", ""));
        }
    }
    var cartTotal = document.getElementById("cart-total");
    cartTotal.innerHTML = sum.toLocaleString("vi-VN") + " VNĐ";
}
var changeQuality = function (input) {
    console.log(input);
    var soLuongTonElement = input.parentElement.querySelector('.soLuongSpTon');

    var soluong = parseInt(input.value) || 1;
    if (soluong > soLuongTonElement.value) {
        showToast("warning", "Vượt quá số lượng tồn kho");
        soluong = soLuongTonElement.value;
        input.value = soLuongTonElement.value;
    } else {
        if (soluong <= 0) {
            input.value = 1;
            soluong = 1;
        }
        else {
            var maSpCart = soLuongTonElement.previousElementSibling;
            updateSoLuongOnServer(maSpCart, soluong);
        }
    }
    //cập nhật giá mới
    var gia = input.parentElement.parentElement.previousElementSibling;
    var sum = input.parentElement.parentElement.nextElementSibling;
    sum.innerHTML = (soluong * Number(gia.innerHTML.replace("VNĐ", "").replaceAll(".", ""))).toLocaleString("vi-VN") + " VNĐ";
    update_SumPrice();
}

var removeProduct = function (btn) {
    showLoading("");
    var maSpCart = btn.previousElementSibling;
    var cart_container = document.getElementById("cart-container");
    $.ajax({
        url: "/Customer/Cart/RemoveProduct",
        type: "POST",
        data: { maSp: maSpCart.value },
        success: function (response) {
            if (response.success) {
                $.ajax({
                    url: "/Customer/Cart/Index",
                    type: "GET",
                    success: function (response) {
                        cart_container.innerHTML = ''; // Xóa các kết quả cũ
                        cart_container.innerHTML = response; // Hiển thị ô kết quả
                    },
                    error: function (xhr, status, error) {
                        console.error("Lỗi :", error);
                    }
                })
                hideLoading();
            } else {
                showToast('error', response.message);
                hideLoading();
            }
        },
        error: function (xhr, status, error) {
            console.error("Lỗi :", error);
        }
    })
}
var chooseAllProduct = function (input) {
    var lstcheckbox = document.getElementsByClassName("checkbox");
    if (input.checked) {
        for (var i = 0; i < lstcheckbox.length; i++) {
            lstcheckbox[i].checked = true;
        }
    } else {
        for (var i = 0; i < lstcheckbox.length; i++) {
            lstcheckbox[i].checked = false;
        }
    }
}

var OrderProductFromCart = function (btn) {
    lstcheckbox = document.getElementsByClassName("checkbox");

    lstSp = [];
    for (var idx = 0; idx < lstcheckbox.length; idx++) {
        if (lstcheckbox[idx].checked) {
            var maSp = lstcheckbox[idx].value;
            var tenSp = document.getElementsByClassName("tensp-" + maSp)[0]?.value;
            var soLuongSpTon = document.getElementsByClassName("soLuongSpTon-" + maSp)[0]?.value;
            
            var soLuong = parseInt(document.getElementsByClassName("soluong-" + maSp)[0]?.value);
            console.log(document.getElementsByClassName("soluong-" + maSp)[0]?.value)
            if (soLuong > soLuongSpTon) {
                showToast("warning", tenSp+" vượt quá số lượng tồn kho");
                return;
            }
            var maSp = lstcheckbox[idx].value;
            var product = {
                MaSanPham: maSp,
                SoLuongDatMua: soLuong
            };
            lstSp.push(product);
        }
    }
    if (lstSp.length <= 0) {
        showToast("warning", "Bạn chưa chọn sản phẩm nào")
    } else {
        var inputElement = document.getElementById("dataSubmit");
        inputElement.value = JSON.stringify(lstSp);
        console.log(inputElement.value);
        var form = document.getElementsByClassName("cart-actions")[0];
        form.submit();
        console.log(lstSp);
    }
}