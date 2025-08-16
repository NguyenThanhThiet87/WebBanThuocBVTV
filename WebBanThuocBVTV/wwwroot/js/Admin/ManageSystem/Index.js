var systemModal = document.getElementById("systemModal");
var modal = new bootstrap.Modal(systemModal);
var modalContainer = document.getElementById("modal-container");

document.addEventListener("DOMContentLoaded", function () {
    showLoading();
    searchCategory();
    hideLoading();
})
var showSection = function (action, btn) {
    var products = document.getElementById("products");
    var manufacturers = document.getElementById("manufacturers");
    if (action == "category") {
        btn.classList.add("active");
        btn.nextElementSibling.classList.remove("active");
        manufacturers.classList.remove("active");
        products.classList.add("active")
        showLoading();
        searchCategory();
        hideLoading();
    } else if (action == "manufacturers") {
        btn.classList.add("active");
        btn.previousElementSibling.classList.remove("active");
        manufacturers.classList.add("active");
        var products = document.getElementById("products");
        products.classList.remove("active")

        showLoading();
        searchManu();
        hideLoading();
    }
}
//NSP
var addCategoryModal = function () {
    $.ajax({
        type: "POST",
        url: "/Admin/ManageSystem/AddCategoryModal",
        data: {},
        success: function (data) {
            modal.show();
            modalContainer.innerHTML = data;
        },
        error: function (error) {
            console.error("Error fetching edit category modal:", error);
        }
    });
}

var addCategory = function () {
    showLoading("");

    var tennsp = document.getElementById("tenNhomSp").value;
    if (tennsp == "") {
        showToast("error", "Dữ liệu không hợp lệ");
        hideLoading();
        return;
    }
    $.ajax({
        method: "POST",
        url: "/Admin/ManageSystem/AddCategory",
        data: { name: tennsp },
        success: function (res) {
            if (res.type == "success") {
                modal.hide();
                searchCategory();
            }
            hideLoading();
            showToast(res.type, res.message);
        },
        error: function (error) {
            showToast("error", error.message);
            hideLoading();
        }
    });
}

var editCategory = function (id) {
    $.ajax({
        method: "POST",
        url: "/Admin/ManageSystem/EditCategoryModal",
        data: { id: id },
        success: function (data) {
            modal.show();
            modalContainer.innerHTML = data;
        },
        error: function (error) {
            console.error("Error fetching edit category modal:", error);
        }
    });
}

var updateCategory = function () {
    showLoading("");
    var mansp = document.getElementById("maNhomSp").value;
    var tennsp = document.getElementById("tenNhomSp").value;
    if (tennsp == "") {
        showToast("error", "Dữ liệu không hợp lệ");
        hideLoading();
        return;
    }
    $.ajax({
        method: "POST",
        url: "/Admin/ManageSystem/EditCategory",
        data: { maNhomSp: mansp, tenNhomSp: tennsp },
        success: function (res) {
            if (res.type == "success") {
                modal.hide();
                hideLoading();
                searchCategory();
            }
            showToast(res.type, res.message);
        },
        error: function (error) {
            showToast("error", error.message);
            hideLoading();
        }
    });
}

var searchCategory = function () {
    var key = document.getElementById("productSearch").value;
    var lstContain = document.getElementsByClassName("table-lstNsp")[0];
    console.log(key);
    $.ajax({
        method: "POST",
        url: "/Admin/ManageSystem/SearchCategory",
        data: { key: key },
        success: function (res) {
            lstContain.innerHTML = res;
        },
        error: function (error) {
            showToast("error", error.message);
        }
    });
}

var deleteCategory = function (id) {
    showComfirm("Bạn có chắc muốn xóa?", "Nhóm sản phẩm không tham chiếu mới có quyền xóa!", () => {
        $.ajax({
            method: "POST",
            url: "/Admin/ManageSystem/DeleteCategory",
            data: { id: id },
            success: function (res) {
                if (res.type == "success") {
                    searchCategory();
                }
                showToast(res.type, res.message);
            },
            error: function (error) {
                showToast("error", error.message);
            }
        });
    })
}
//NSX
var addManuModal = function () {
    $.ajax({
        type: "POST",
        url: "/Admin/ManageSystem/AddManuModal",
        data: {},
        success: function (data) {
            modal.show();
            modalContainer.innerHTML = data;
        },
        error: function (error) {
            console.error("Error fetching edit category modal:", error);
        }
    });
}

var addManu = function () {
    showLoading("");

    var tennsx = document.getElementById("tenNhaSx").value;
    if (tennsx == "") {
        showToast("error", "Dữ liệu không hợp lệ");
        hideLoading();
        return;
    }

    $.ajax({
        method: "POST",
        url: "/Admin/ManageSystem/AddManu",
        data: { name: tennsx },
        success: function (res) {
            if (res.type == "success") {
                modal.hide(); 
                searchManu();
            }
            hideLoading();
            showToast(res.type, res.message);
        },
        error: function (error) {
            showToast("error", error.message);
            hideLoading();
        }
    });
}

var editManu = function (id) {
    $.ajax({
        method: "POST",
        url: "/Admin/ManageSystem/EditManuModal",
        data: { id: id },
        success: function (data) {
            modal.show();
            modalContainer.innerHTML = data;
        },
        error: function (error) {
            console.error("Error fetching edit category modal:", error);
        }
    });
}

var updateManu = function () {
    showLoading("");
    var mansx = document.getElementById("maNhaSx").value;
    var tennsx = document.getElementById("tenNhaSx").value;

    if (tennsx == "") {
        showToast("error", "Dữ liệu không hợp lệ");
        hideLoading();
        return;
    }
    $.ajax({
        method: "POST",
        url: "/Admin/ManageSystem/EditManu",
        data: { maNhaSx: mansx, tenNhaSx: tennsx },
        success: function (res) {
            if (res.type == "success") {
                modal.hide();
                hideLoading();
                searchManu();
            }
            showToast(res.type, res.message);
        },
        error: function (error) {
            showToast("error", error.message);
            hideLoading();
        }
    });
}

var searchManu = function () {
    var key = document.getElementById("manufacturerSearch").value;
    var lstContain = document.getElementsByClassName("table-lstNsx")[0];
    console.log(key);
    $.ajax({
        method: "POST",
        url: "/Admin/ManageSystem/SearchManu",
        data: { key: key },
        success: function (res) {
            console.log(res)
            lstContain.innerHTML = res;
        },
        error: function (error) {
            showToast("error", error.message);
        }
    });
}

var deleteManu = function (id) {
    showComfirm("Bạn có chắc muốn xóa?", "Nhà sản xuất không tham chiếu mới có quyền xóa!", () => {
        $.ajax({
            method: "POST",
            url: "/Admin/ManageSystem/DeleteManu",
            data: { id: id },
            success: function (res) {
                if (res.type == "success") {
                    searchManu();
                }
                showToast(res.type, res.message);
            },
            error: function (error) {
                showToast("error", error.message);
            }
        });
    })
}