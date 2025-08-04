var commentModal = document.getElementById("commentModal");
var modal = new bootstrap.Modal(commentModal);
var modalContainer = document.getElementById("modal-container");

commentModal.addEventListener('hide.bs.modal', function (event) {
    applyFilters();
});

document.addEventListener("DOMContentLoaded", () => {
    applyFilters();
})

var applyFilters = function () {
    showLoading("");
    var name = document.getElementById("searchInput").value;
    var evaluate = document.getElementsByClassName("evaluate-filter")[0].value;
    var isReply = document.getElementsByClassName("reply-filter")[0].value;
    var state = document.getElementsByClassName("state-filter")[0].value;
    console.log(evaluate)
    var tableProduct = document.getElementsByClassName("comments-table")[0];

    $.ajax({
        url: "/Admin/ManageComment/FilterComment",
        method: "POST",
        data: { keyword: name, evaluateOptions: evaluate, isReplyOptions: isReply, stateOptions: state },
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
var replyCommentModal = function (thoiGian, maNd, maSp) {
    var modal_container = document.getElementById("modal-container");

    var dataForm = {
        maNd: maNd,
        maSanPham: maSp,
        thoiGian: thoiGian
    }
    $.ajax({
        url: "/Admin/ManageComment/ReplyComment",
        method: "POST",
        data: { bl: dataForm },
        success: function (res) {
            hideLoading()
            modal_container.innerHTML = res;
        },
        error: function (err) {
            hideLoading()
            console.log(err);
        }
    })
}
var reply = function (thoiGian, maNd, maSp, maNv) {
    showComfirm("Bạn có chắc muốn gửi?", "Hãy cân nhắc trước khi thực hiện", () => {
        showLoading("");
        var maNhanVien = maNv;
        var noiDung = document.getElementById("replyContent").value;

        var dataForm = {
            MaNdBinhLuan: maNd,
            MaSpBinhLuan: maSp,
            ThoiGianBinhLuan: thoiGian,
            MaNhanVien: maNhanVien,
            NoiDungPhanHoi: noiDung
        }

        $.ajax({
            url: "/Admin/ManageComment/Reply",
            method: "POST",
            data: { ph: dataForm },
            success: function (res) {
                hideLoading()
                if (res.type == "success") {
                    modal.hide();
                }
                showToast(res.type, res.message);
            },
            error: function (err) {
                hideLoading()
                console.log(err);
            }
        })
    })
}

var deleteComment = function (maPh) {
    showComfirm("Bạn có chắc muốn xóa?", "Hãy cân nhắc trước khi thực hiện", () => {
        showLoading();
        $.ajax({
            url: "/Admin/ManageComment/DeleteReply",
            method: "POST",
            data: { maPh: maPh },
            success: function (res) {
                hideLoading()
                if (res.type == "success") {
                    applyFilters();
                }
                showToast(res.type, res.message);
            },
            error: function (err) {
                hideLoading()
                console.log(err);
            }
        })
    })

}

var EditReplyModal = function (maPh) {
    var modal_container = document.getElementById("modal-container");

    $.ajax({
        url: "/Admin/ManageComment/EditReplyComment",
        method: "POST",
        data: { maPh: maPh },
        success: function (res) {
            hideLoading()
            modal_container.innerHTML = res;
        },
        error: function (err) {
            hideLoading()
            console.log(err);
        }
    })
}
var EditReply = function (maPh, maNv) {
    showComfirm("Bạn có chắc muốn cập nhật?", "Hãy cân nhắc trước khi thực hiện", () => {
        showLoading("");
        var noiDung = document.getElementById("replyContent").value;

        $.ajax({
            url: "/Admin/ManageComment/EditReply",
            method: "POST",
            data: {
                maPh: maPh, 
                maNv: maNv,
                noiDung: noiDung
                },
            success: function (res) {
                hideLoading()
                if (res.type == "success") {
                    modal.hide();
                }
                showToast(res.type, res.message);
            },
            error: function (err) {
                hideLoading()
                console.log(err);
            }
        })
    })
}