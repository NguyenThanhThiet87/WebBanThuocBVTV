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
        var funct = null;
        switch (type) {
            case "success":
                funct = iziToast.success({
                    title: 'Success',
                    message: message,
                    position: "topLeft",
                    theme: 'light'
                });
                break;
            case "warning":
                iziToast.warning({
                    title: 'Warning',
                    message: message,
                    position: "topLeft",
                    theme: 'light'
                });
                break;
            case "error":
                iziToast.error({
                    title: 'Error',
                    message: message,
                    position: "topLeft",
                    theme: 'light'
                });
                break;
            default:
                funct = iziToast.success({
                    title: 'Success',
                    message: message,
                    position: "topLeft",
                    theme: 'light'
                });
        }
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
