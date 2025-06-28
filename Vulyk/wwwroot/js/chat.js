const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub").build();

var chatLauncher;
var selectedChatList;
async function startConnectionAndRun() {
    await connection.start();
    if (document.readyState == 'loading') {
        document.addEventListener('DOMContentLoaded', async () => {
            await run();
        });
    } else {
        await run();
    }
}
async function run() {
    chatLauncher = document.getElementById('chat-launcher');
    if (chatLauncher.dataset.chatId) {
        chooseChat(chatLauncher.dataset.chatId, chatLauncher.dataset.userToAddId);
    } else if (chatLauncher.dataset.userToAddId) {
        createEmptyChat(chatLauncher.dataset.userToAddId);
    }
    await connection.invoke('JoinUserGroupAsync', chatLauncher.dataset.yourUserId);

    await connection.invoke("LoadChatsAsync", JSON.parse(chatLauncher.dataset.chatIds));
}
startConnectionAndRun();

connection.on('ReceiveMessage', (userId, message) => {
    updateChatList(userId, null, message, false);
});

connection.on('CreateChat', async (userId, chatId, name, lastMessage) => {
    await connection.invoke("LoadChatAsync", chatId.toString());
    var noChatsDiv = document.getElementById('no-chats');
    if (noChatsDiv) {
        noChatsDiv.classList.add('newUserAdded');
    }
    createChatListItem(userId, chatId, lastMessage, name, getCurrentTime());
    const chatIdDiv = document.getElementById('chatId');
    if (chatIdDiv) {
        chatIdDiv.dataset.chatId = chatId;
    }
});

function chooseChat(chatId, userId) {
    fetch(`Message/Index?chatId=${chatId}&partnerUserId=${userId}`)
        .then(r => r.text())
        .then(html => {
            document.getElementById('messages').innerHTML = html;
            scrollMessageContainer();
        });
    if (selectedChatList) {
        selectedChatList.classList.remove('selected');
    }
    changeChatItemColor(chatId);
}

function changeChatItemColor(chatId) {
    var newSelectedChatList = document.querySelector(`.chat-item[data-chat-id="${chatId}"]`);
    newSelectedChatList.classList.add('selected');
    selectedChatList = newSelectedChatList;
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
        var chatId, userId;
        if (e.target.classList.contains('chat-item')) {
            chatId = e.target.dataset.chatId;
            userId = e.target.dataset.userId;
        } else if (e.target.closest('.chat-item')) {
            chatId = e.target.closest('.chat-item').dataset.chatId;
            userId = e.target.closest('.chat-item').dataset.userId;
        }
        if (chatId == null) {
            return;
        }
        chooseChat(chatId, userId);
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
            const chatIdDiv = document.getElementById('chatId');
            var chatId = chatIdDiv.dataset.chatId;
            var userId = formData.get('UserId');
            var yourUserId = chatLauncher.dataset.yourUserId;
            var name = chatLauncher.dataset.yourName;
            var text = formData.get("text");
            if (chatId == '') {
                var result = await r.json();
                chatId = result.chatId;
                name = result.name;
                chatIdDiv.dataset.chatId = chatId;
                await connection.invoke("CreateChatAsync", userId, parseInt(yourUserId), chatId, name, text);
            } else {
                try {
                    await connection.invoke("SendMessageAsync", chatId.toString(), parseInt(yourUserId), text);
                }
                catch (er) {
                    console.log(er);
                }
            }

            updateChatList(userId, chatId, formData.get("text"), true, name);
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
    const messageContainer = document.querySelector('.message-container');
    if (messageContainer == null) {
        return;
    }
    messageContainer.scrollTop = messageContainer.scrollHeight;
}

function updateChatList(userId, chatId, message, isYourMessage, userName) {
    try {
        if (chatLauncher.dataset.yourUserId == userId) {
            return;
        }
        var chatItem = document.querySelector(`.chat-item[data-user-id='${userId}']`);
        const currentTime = getCurrentTime();
        if (chatItem == null) {
            createChatListItem(userId, chatId, message, userName, currentTime);
            changeChatItemColor(chatId);
        } else {
            var lastMessageText = chatItem.querySelector('.chat-last-message');
            lastMessageText.textContent = message;

            var lastMessageDateTime = chatItem.querySelector('.last-message-data-time');

            lastMessageDateTime.textContent = currentTime;
        }

        appendMessage(currentTime, userId, message, isYourMessage);
    }
    catch (err) {
        console.log(err);
    }
}

function createChatListItem(userId, chatId, lastMessageText, userName, currentTime) {
    const chatPanelDiv = document.querySelector('.chat-panel');

    const chatItemDiv = document.createElement('div');
    chatItemDiv.classList.add('chat-item');
    chatItemDiv.dataset.chatId = chatId;
    chatItemDiv.dataset.userId = userId;
    chatPanelDiv.prepend(chatItemDiv);

    const horizontalDiv = document.createElement('div');
    horizontalDiv.classList.add('horizontal');
    chatItemDiv.appendChild(horizontalDiv);

    const nameDiv = document.createElement('div');
    nameDiv.classList.add('chat-title');
    nameDiv.textContent = userName;
    horizontalDiv.appendChild(nameDiv);

    const lastMessageDateTimeDiv = document.createElement('div');
    lastMessageDateTimeDiv.classList.add('last-message-data-time');
    lastMessageDateTimeDiv.classList.add('secondary-text');
    lastMessageDateTimeDiv.textContent = currentTime;
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
    var messageContainer = document.querySelector(`.message-container[data-user-id='${userId}']`);
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