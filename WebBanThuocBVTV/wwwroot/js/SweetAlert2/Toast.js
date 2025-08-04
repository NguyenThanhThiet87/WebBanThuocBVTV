const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    iconColor: 'white',
    customClass: {
        popup: 'colored-toast',
    },
    showConfirmButton: false,
    timer: 2500,
    timerProgressBar: true,
});
function showToast(type, message) {
    setTimeout(() => {
        Toast.fire({
            icon: type,
            title: message,
        });
    }, 100);
}
function showLoading(message) {
    Swal.fire({
        title: message || 'Đang xử lý...', // Dùng message được truyền vào, hoặc mặc định
        html: 'Vui lòng chờ trong giây lát.',
        timerProgressBar: true,
        allowOutsideClick: false, // Ngăn người dùng tắt bằng cách bấm ra ngoài
        didOpen: () => {
            Swal.showLoading(); // Hiển thị icon xoay tròn
        }
    });
}

function hideLoading() {
    Swal.close();
}

function showComfirm(title, text, comfFunc) {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            actions: 'gap-3',
            confirmButton: "btn btn-success",
            cancelButton: "btn btn-danger"
        },
        buttonsStyling: false
    });
    swalWithBootstrapButtons.fire({
        title: title,
        text: text,
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Đồng ý",
        cancelButtonText: "No, Thoát",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            comfFunc()
        } else if (result.dismiss === Swal.DismissReason.cancel) {

        }
    });
}
