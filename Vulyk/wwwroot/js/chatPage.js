function chooseChat(chatId) {
    fetch(`Message/Index?chatId=${chatId}`)
        .then(r => r.text())
        .then(html => document.getElementById('messages').innerHTML = html)
}

function CreateMessage(chatId, text) {
    fetch(`Message/CreateMessage?chatId=${chatId}&text=${text}`)
        .then(r => r.text())
        .then(t => document.getElementById();
}