const radios = document.querySelectorAll('input[name="addType"]');
const loginInput = document.getElementById('loginInput');
const phoneInput = document.getElementById('phoneInput');
radios.forEach(r => {
    r.addEventListener('change', () => {
        if (r.value === 'login' && r.checked) {
            phoneInput.classList.add('hidden');
            loginInput.classList.remove('hidden');
        } else {
            loginInput.classList.add('hidden');
            phoneInput.classList.remove('hidden');
        }
    }
    )
}
);