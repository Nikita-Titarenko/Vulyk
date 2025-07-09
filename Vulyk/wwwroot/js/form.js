var passwordVisible = false;
var eyeIcon;
var passwordInput;
document.addEventListener('DOMContentLoaded', (e) => {
    eyeIcon = document.querySelector('.eye-icon');
    passwordInput = document.querySelector('.password-input');
    document.querySelector('.btn-show-password').addEventListener('click', () => {
        if (passwordVisible) {
            passwordVisible = false;
            eyeIcon.classList.add('fa-eye-slash');
            eyeIcon.classList.remove('fa-eye');
            passwordInput.type = 'password';
        } else {
            passwordVisible = true;
            eyeIcon.classList.add('fa-eye');
            eyeIcon.classList.remove('fa-eye-slash');
            passwordInput.type = 'text';
        }
    });
});