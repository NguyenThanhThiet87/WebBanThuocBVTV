var modal_container = document.getElementById("modal-container");
var verifyModal = document.getElementById("verifyModal");
var modal = new bootstrap.Modal(verifyModal);

var sendOTPEmail = function () {
	showLoading("");

	var email = document.getElementById("email").value;

	$.ajax({
		url: "/Admin/ManageCustomer/SendOTPEmail",
		method: "post",
		data: { email: email },
		success: function (res) {
			hideLoading();
			if (res.success == null) {
				modal_container.innerHTML = res;
				modal.show();
			} else {
				showToast('warning', res.message);
			}

		},
		error: function (res) {
			hideLoading();
			showToast('error', res.message);
		}
	})
}
var verifyEmail = function () {
	var email = document.getElementById("email").value;

	var otp = ""
	var inputs = document.getElementsByClassName("otp-input");

	for (var i = 0; i < inputs.length; i++) {
		otp += inputs[i].value;
	}

	$.ajax({
		url: "/Admin/ManageCustomer/VerifyOTPEmail",
		method: "post",
		data: { otp: otp, email: email },
		success: function (res) {
			hideLoading();
			if (res.success) {
				showToast('success', res.message);
				var verifyBtn = document.getElementById("verifyEmailBtn");
				verifyBtn.style.display = "none";
				modal.hide();
			} else {
				showToast('warning', res.message);
			}
		},
		error: function (res) {
			hideLoading();
			showToast('error', res.message);
		}
	}
	)
}
var reviewImage = function (input) {

	var file = input.files[0];
	var imgElement = document.getElementsByClassName("imgAvatar")[0];
	var iconElement = document.getElementsByClassName("iconAvatarCustomer")[0];
	iconElement.classList.add("d-none");
	imgElement.classList.remove("d-none");
	imgElement.src = URL.createObjectURL(file);
}