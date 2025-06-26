const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub").build();

const promise = connection.start();

connection.on('ReceiveMessage', (userId, message) => {
    updateChatList(userId, message, false);
});

function loadChats(chatIds) {
    promise.then(() => {
        connection.invoke("LoadChats", chatIds)
    });
}

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

const chatPanel = document.querySelector('.chat-panel');
if (chatPanel) {
    chatPanel.addEventListener('click', function (e) {
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
}


function CreateMessage(e) {
    e.preventDefault();
    const form = e.target;
    const formData = new FormData(form);
    fetch(`Message/CreateMessage`, {
        method: 'POST',
        body: formData
    }).then(async r => {
        if (r.ok) {
            const inputChatId = document.getElementById('chatId');
            var chatId = inputChatId.value;
            if (chatId == '') {
                chatId = formData.get('ChatId');
                if (chatId == '') {
                    var chatId = await r.json();
                    inputChatId.value = chatId;
                    await promise;
                    await connection.invoke("CreateChat", chatId.toString());
                }
            }
            await promise;
            try {
                var userId = formData.get("UserId");
                var text = formData.get("text");
                await connection.invoke("SendMessage", chatId.toString(), parseInt(userId), text);
            }
            catch (er) {
                console.log(er);
            }

            updateChatList(formData.get("UserId"), formData.get("text"), true);
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

function updateChatList(userId, message, isYourMessage) {
    try {
        const chatLauncher = document.getElementById('chat-launcher');

        if (chatLauncher.dataset.yourUserId == userId) {
            return;
        }
        var chatItem = document.querySelector(`[data-userid='${userId}']`);
        if (chatItem == null) {
            createChatList(userId, message);
        } else {
            var lastMessageText = chatItem.querySelector('.chat-last-message');
            lastMessageText.textContent = message;

            var lastMessageDateTime = chatItem.querySelector('.last-message-data-time');
            const currentTime = getCurrentTime();
            lastMessageDateTime.textContent = currentTime;
        }

        appendMessage(currentTime, userId, message, isYourMessage);
    }
    catch (err) {
        console.log(err);
    }
}

function createChatList(userId, chatId, lastMessageText) {
    const chatPanelDiv = document.getElementById('chat-panel');
    chatPanelDiv.prepend(chatItemDiv);

    const chatItemDiv = document.createElement('div');
    chatItemDiv.classList.add('chat-item');
    chatItemDiv.dataset.add('data-chatid', 0);
    chatItemDiv.dataset.add('data-userid', userId);

    const horizontalDiv = document.createElement('div');
    horizontalDiv.classList.add('horizontal');
    chatItemDiv.appendChild(horizontalDiv);

    const nameDiv = document.createElement('div');
    nameDiv.classList.add('chat-title');
    horizontalDiv.appendChild(nameDiv);

    const lastMessageDateTimeDiv = document.createElement('div');
    lastMessageDateTimeDiv.classList.add('last-message-data-time');
    lastMessageDateTimeDiv.classList.add('secondary-text');
    lastMessageDateTimeDiv.textContent = getCurrentTime();
    horizontalDiv.appendChild(lastMessageDateTimeDiv);

    const lastMessageTextDiv = document.createElement('div');
    lastMessageTextDiv.classList.add('chat-title');
    lastMessageTextDiv.classList.add('chat-last-message');
    lastMessageTextDiv.classList.add('secondary-text');
    lastMessageTextDiv.textContent = lastMessageText;
    chatItemDiv.appendChild(lastMessageTextDiv);
}

function getCurrentTime() {
    const dateTime = new Date();
    const hours = dateTime.getHours().toString().padStart(2, '0');
    const minutes = dateTime.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
}

function appendMessage(time, userId, message, isYourMessage) {
    var messageContainer = document.querySelector(`.messageContainer[data-user-id='${userId}']`);
    if (messageContainer) {
        messageDiv = document.createElement('div');
        messageDiv.classList.add('message');
        messageDiv.classList.add(isYourMessage ? 'your' : 'partner');

        const messageTextDiv = document.createElement('div');
        messageTextDiv.textContent = message;
        messageTextDiv.classList.add('message-text');
        messageDiv.appendChild(messageTextDiv);

        const messageTimeDiv = document.createElement('div');
        messageTimeDiv.textContent = time;
        messageTimeDiv.classList.add('message-time');
        messageDiv.appendChild(messageTimeDiv);

        messageContainer.appendChild(messageDiv);

        scrollMessageContainer();
    }
}