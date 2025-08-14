var cancelOrder = function (orderId) {
    showComfirm("Bạn có chắc muốn hủy đơn hàng?", "Hãy cân nhắc trước khi thực hiện", () => {
        showLoading("");

        $.ajax({
            url: "/Customer/Order/CancelOrder",
            method: "POST",
            data: { maDonHang: orderId.trim() },
            success: function (res) {
                hideLoading()
                if (res.type == "success") {
                    showToast('success', 'Hủy đơn thành công! Đang chuyển trang...');
                    // Đợi một chút để người dùng đọc toast rồi mới chuyển trang
                    setTimeout(function () {
                        window.location.href = "/Customer/Home/Index"; // Chuyển hướng đến trang danh sách đơn hàng
                    }, 1500); // Chuyển trang sau 1.5 giây
                } else {
                    showToast(res.type, res.message);
                }
                
            },
            error: function (err) {
                hideLoading()
                console.log(err);
            }
        })
    })
}


