const radios = document.querySelectorAll('input[name="CreateType"]');
const loginInput = document.getElementById('loginInput');
const phoneInput = document.getElementById('phoneInput');

function updateVisibility() {
    const radio = document.querySelector('input[name="CreateType"]:checked');
    if (radio && radio.value === 'Login') {
        phoneInput.classList.add('hidden');
        loginInput.classList.remove('hidden');
    } else {
        loginInput.classList.add('hidden');
        phoneInput.classList.remove('hidden');
    }
}

radios.forEach(r => {
    r.addEventListener('change', updateVisibility);
});

window.addEventListener('DOMContentLoaded', () => {
    updateVisibility();
});