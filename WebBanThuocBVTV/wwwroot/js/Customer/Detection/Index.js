function ChoosePicture(input) {
	var img = document.getElementById("img_uploaded")
	var reader = new FileReader();
	reader.onload = function (e) {
		img.src = e.target.result; // Gán Data URL cho img.src
		// Ẩn các phần tử có class "guide"
		var guides = document.getElementsByClassName("guide")
		for (let guide of guides) {
			guide.style.display = "none";
		}
	};
	reader.readAsDataURL(input.files[0]); // Đọc file đầu tiên
}
var Detect = function () {
	showLoading("");
	var inputFile = document.getElementById("fileInput");
	var file = inputFile.files[0]

	const formData = new FormData();
	formData.append("img", file);
	if (!file) {
		showToast("warning", "Vui lòng chọn ảnh lá cây cần chẩn đoán");
		return;
	}
	$.ajax({
		url: "/Customer/Detection/Detection",
		method: "POST",
		data: formData,
		contentType: false,
		processData: false,
		success: function (res) {
			if (res.success) {
				document.querySelector(".disease-name").textContent = "Khả năng: " + res.message.nameInference;
				document.querySelector(".confidence").textContent = "Độ tin cậy: " + (res.message.confInference * 100) + "%";
				console.log(res.message)
			}
			else {
				showToast("error", res.message);
			}
			hideLoading();
		},
		error: function (error) {
			showToast("error", error);
			hideLoading();
		}
	})
}