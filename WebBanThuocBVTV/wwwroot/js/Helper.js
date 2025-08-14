var togglePass = function (action) {
	var eyeSlash = document.getElementsByClassName("fa-eye-slash")[0];
	var eye = document.getElementsByClassName("fa-eye")[0];
	var password = document.getElementById("password");

	if (action == "display") {
		password.type = "text";
		eye.classList.add("d-none");
		eyeSlash.classList.remove("d-none");
	} else if (action == "hidden") {
		password.type = "password";
		eye.classList.remove("d-none");
		eyeSlash.classList.add("d-none");
	}
}

var nextCell = function (input, event, currentIdx) {
	if (currentIdx != 6) {
		if (event.which != 8 && event.data != null) {
			input.nextElementSibling.focus();
		}
	}
}
var previous = function (input, event, currentIdx) {
	if (currentIdx != 1 && input.value == "") {
		if (event.which == 8) {
			input.focus();
			input.previousElementSibling.focus();
		}
	}
}