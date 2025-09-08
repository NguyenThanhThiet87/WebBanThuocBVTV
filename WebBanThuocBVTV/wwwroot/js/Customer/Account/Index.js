// Tab switching
const tabButtons = document.querySelectorAll('.tab-btn');
const tabContents = document.querySelectorAll('.tab-content');

tabButtons.forEach(button => {
	button.addEventListener('click', () => {
		tabButtons.forEach(btn => btn.classList.remove('active'));
		tabContents.forEach(content => content.classList.remove('active'));

		button.classList.add('active');
		document.getElementById(button.getAttribute('data-tab')).classList.add('active');
	});
});

function togglePassword(inputId, toggleElement) {
	var input = document.getElementById(inputId);
	var toggle = toggleElement;
	if (input.type === "password") {
		input.type = "text";
		toggle.classList.add("active");
	} else {
		input.type = "password";
		toggle.classList.remove("active");
	}
}
var reviewImage = function (input) {
	var file = input.files[0];
	var imgElement = document.getElementsByClassName("imgAvatar")[0];
	var iconElement = document.getElementsByClassName("iconAvatarCustomer")[0];
	iconElement.classList.add("d-none");
	console.log(imgElement);
	imgElement.classList.remove("d-none");
	imgElement.src = URL.createObjectURL(file);
}