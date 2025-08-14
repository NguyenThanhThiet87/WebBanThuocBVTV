function choseStar(sp) {
	var currentRate = parseInt(sp.id);
	document.getElementById("maDg").value = currentRate.toString();
	// Duyệt qua các sao từ 1 đến 5
	for (var i = 1; i <= 5; i++) {
		var star = document.getElementById(i.toString());
		if (i <= currentRate) {
			star.classList.add("active");
		} else {
			star.classList.remove("active");
		}
	}
}
function validateReviewForm(event) {
	var rating = document.getElementById("maDg").value;
	if (!rating || rating === "0" || parseInt(rating) < 1 || parseInt(rating) > 5) {
		event.preventDefault();
		alert("Vui lòng đánh giá sản phẩm");
		return false; // Ngăn form gửi đi
	}
	return true; // Cho phép gửi form
}