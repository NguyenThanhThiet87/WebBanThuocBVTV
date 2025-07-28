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
    Toast.fire({
        icon: type,
        title: message,
    });
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
