let passwordVisible = false;
let eyeIcons;
let passwordInputs;

eyeIcons = document.querySelectorAll('.eye-icon');
passwordInputs = document.querySelectorAll('.password-input');
document.querySelectorAll('.btn-show-password').forEach(b => b.addEventListener('click', () => {
    if (passwordVisible) {
        passwordVisible = false;
        eyeIcons.forEach(b => b.classList.add('fa-eye-slash'));
        eyeIcons.forEach(b => b.classList.remove('fa-eye'));
        passwordInputs.forEach(b => b.type = 'password');
    } else {
        passwordVisible = true;
        eyeIcons.forEach(b => b.classList.add('fa-eye'));
        eyeIcons.forEach(b => b.classList.remove('fa-eye-slash'));
        passwordInputs.forEach(b => b.type = 'text');
    }
}));