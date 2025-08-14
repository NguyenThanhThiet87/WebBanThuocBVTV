var verifyModal = document.getElementById("verifyModal");
var modal = new bootstrap.Modal(verifyModal);
var modalContainer = document.getElementById("modal-container");

var sendOTP = function () {
	showLoading("");

	var email = document.getElementById("email").value;
	if (email == "" || /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/.test(email.trim()) == false) {
		showToast("warning", "Email không hợp lệ");
		hideLoading();
		return;
	}
	$.ajax({
		url: "/Customer/Account/SendOTPEmail",
		method: "POST",
		data: { email },
		success: function (res) {
			modalContainer.innerHTML = res;
			modal.show();
			hideLoading();
		},
		error: function (err) {
			console.log(err);
			hideLoading();
		}
	})
}

var verifyEmail = function () {
	showLoading("");
	var email = document.getElementById("email").value;

	var otp = ""
	var inputs = document.getElementsByClassName("otp-input");

	for (var i = 0; i < inputs.length; i++) {
		otp += inputs[i].value;
	}

	$.ajax({
		url: "/Customer/Account/VerifyOTP",
		method: "post",
		data: { otpCode: otp, email: email },
		success: function (res) {
			hideLoading();
			if (res.success) {
				showToast('success', 'Xác thực thành công! Đang chuyển trang...');
				// Đợi một chút để người dùng đọc toast rồi mới chuyển trang
				setTimeout(function () {
					window.location.href = res.redirectUrl;
				}, 1500); // Chuyển trang sau 1.5 giây

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

var resendOTP = function () {
	showLoading("");

	var email = document.getElementById("email").value;
	if (email == "" || /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/.test(email.trim()) == false) {
		showToast("warning", "Email không hợp lệ");
		console.log(modal);
		hideLoading();
		return;
	}
	$.ajax({
		url: "/Customer/Account/SendOTPEmail",
		method: "POST",
		data: { email },
		success: function (res) {
			hideLoading();
		},
		error: function (err) {
			console.log(err);
			hideLoading();
		}
	})
}