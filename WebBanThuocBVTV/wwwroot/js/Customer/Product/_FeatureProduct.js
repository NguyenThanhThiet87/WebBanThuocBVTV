document.addEventListener("DOMContentLoaded", function () {
    var container = document.getElementsByClassName("feature-product")[0];
    $.ajax({
        url: "/Customer/Product/FeaturedProduct/",
        method: "GET",
        success: function (res) {
            container.innerHTML = res;
        },
        error: function (error) {
            console.error("Lỗi khi tải sản phẩm:", error);
        }
    })
});