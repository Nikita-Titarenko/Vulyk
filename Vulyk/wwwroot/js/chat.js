function chooseChat(chatId) {
    fetch(`Message/Index?chatId=${chatId}`)
        .then(r => r.text())
        .then(html => {
            document.getElementById('messages').innerHTML = html;
            scrollMessageContainer();
        });
}

document.querySelector('.chat-panel').addEventListener('click', function (e) {
    if (!e.target) {
        return;
    }
    var chatId;
    if (e.target.classList.contains('chat-item')) {
        chatId = e.target.dataset.chatid;
    } else if (e.target.closest('.chat-item')) {
        chatId = e.target.closest('.chat-item').dataset.chatid;
    }
    if (chatId == null) {
        return;
    }
    chooseChat(chatId);
});

function CreateMessage(e) {
    e.preventDefault();
    const form = e.target;
    const formData = new FormData(form);
    fetch(`Message/CreateMessage`, {
        method: 'POST',
        body: formData
    }).then(r => {
        if (r.ok) {
            form.reset();
        }
    });
}
document.addEventListener('submit', (e) => {
    if (e.target && e.target.id) {
        CreateMessage(e);
    }
})

function scrollMessageContainer() {
    const messageContainer = document.querySelector('.messageContainer');
    if (messageContainer == null) {
        return;
    }
    messageContainer.scrollTop = messageContainer.scrollHeight;
}