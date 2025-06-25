function chooseChat(chatId) {
    fetch(`Message/Index?chatId=${chatId}`)
        .then(r => r.text())
        .then(html => {
            document.getElementById('messages').innerHTML = html;
            scrollMessageContainer();
        });
}

function createEmptyChat(userId) {
    fetch(`Message/DisplayEmptyChat?userId=${userId}`)
        .then(r => r.text())
        .then(html => {
            document.getElementById('messages').innerHTML = html;
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

document.addEventListener('DOMContentLoaded', () => {
    const chatLauncher = document.getElementById('chat-launcher');
    if (chatLauncher.dataset.userToAddId) {
        createEmptyChat(chatLauncher.dataset.userToAddId);
    } else if (chatLauncher.dataset.chatId) {
        chooseChat(chatLauncher.dataset.chatId);
    }

    loadChats(JSON.parse(chatLauncher.dataset.chatIds));
});

function updateChatList(chatId, userId, message) {
    try {
        var chatItem = document.querySelector(`[data-chatid='${chatId}']`);
        var lastMessageText = chatItem.querySelector('.chat-last-message');
        lastMessageText.textContent = message;
        var lastMessageDateTime = chatItem.querySelector('.last-message-data-time');
        const dateTime = new Date();
        const hours = dateTime.getHours().toString().padStart(2, '0');
        const minutes = dateTime.getMinutes().toString().padStart(2, '0');
        const time = `${hours}:${minutes}`;
        lastMessageDateTime.textContent = time;
        appendMessage(time, userId, message);
    }
    catch (err) {
        console.log(err);
    }
}

function appendMessage(time, userId, message) {
    var messageContainer = document.querySelector(`.messageContainer[data-user-id='${userId}']`);
    if (messageContainer) {
        const yourUserId = document.querySelector(`[data-your-user-id='${userId}']`).textContent;

        messageDiv = document.createElement('div');
        messageDiv.classList.add('message');
        messageDiv.classList.add(userId == yourUserId ? 'your' : 'partner');

        const messageTextDiv = document.createElement('div');
        messageTextDiv.textContent = message;
        messageTextDiv.classList.add('message-text');
        messageDiv.appendChild(messageTextDiv);

        const messageTimeDiv = document.createElement('div');
        messageTimeDiv.textContent = time;
        messageTimeDiv.classList.add('message-time');
        messageDiv.appendChild(messageTimeDiv);

        messageContainer.appendChild(messageDiv);
    }
}

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub").build();
function loadChats(chatIds) {
    connection.start().then(() => {
        connection.invoke("LoadChats", chatIds)
    });
    connection.on('ReceiveMessage', (chatId, userId, message) => {
        updateChatList(chatId, userId, message);
    });
}