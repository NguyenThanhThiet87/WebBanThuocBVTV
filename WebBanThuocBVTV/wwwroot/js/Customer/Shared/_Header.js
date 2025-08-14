// Biến để quản lý thời gian chờ (debounce)
let debounceTimeout;

// Hàm tìm kiếm đã được viết
function performSearch(input) {
    let keyword = input.value;
    let resultsList = document.getElementById('resultsContainer');

    if (keyword == "") {
        resultsList.classList.add("hidden");
        return;
    }
    resultsList.classList.remove("hidden");
    resultsList.innerHTML = '<div class="spinner"> <div class="cube1"></div> <div class="cube2"></div> </div>';
    // Hủy bỏ yêu cầu tìm kiếm trước đó nếu người dùng vẫn đang gõ
    clearTimeout(debounceTimeout);
    // Điều này giúp giảm tải cho server, chỉ tìm khi người dùng ngừng gõ
    debounceTimeout = setTimeout(() => {
        $.ajax({
            url: "/Customer/Product/SearchProduct", // Đảm bảo đúng đường dẫn
            type: "POST",
            data: { keyword: keyword },
            success: function (response) {
                resultsList.innerHTML = ''; // Xóa các kết quả cũ
                resultsList.innerHTML = response // Hiển thị ô kết quả
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi tìm kiếm:", error);
                resultsList.classList.add("hidden");
            }
        });
    }, 300);
}
document.addEventListener('click', (e) => {
    if (!e.target.closest('#resultsContainer')) {
        let resultsList = document.getElementById('resultsContainer');
        resultsList.classList.add("hidden");
    }
})