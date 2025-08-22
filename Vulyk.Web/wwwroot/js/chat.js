const connection = new signalR.HubConnectionBuilder()
    .withAutomaticReconnect()
    .withUrl("/chathub").build();

connection.onreconnecting((error) => {
    showToast('Connection lost. Trying to reconnect...', colors['error']);
});

connection.onreconnected(async (error) => {
    showToast('Connection restored', colors['success'], 5000);
    await addToGroups();
});

connection.onclose(async (error) => {
    showToast('Connection closed', colors['error']);
});

const chatLauncher = document.getElementById('chat-launcher');;
let selectedChatList;
let noSelectedChatMessage = document.getElementById('no-selected-chat');

async function startConnection() {
    try {
        await connection.start();
        await addToGroups();
        await loadChat();
    }
    catch (error) {
        showToast('Failed to load chats. Please try again later.', colors['error']);
    }
}

async function addToGroups() {
    await connection.invoke('JoinUserGroupAsync', chatLauncher.dataset.yourUserId);
    await connection.invoke("LoadChatsAsync", JSON.parse(chatLauncher.dataset.chatIds));
}

async function loadChat() {
    if (chatLauncher.dataset.chatId) {
        await chooseChat(chatLauncher.dataset.chatId, chatLauncher.dataset.userToAddId);
    } else if (chatLauncher.dataset.userToAddId) {
        await createEmptyChat(chatLauncher.dataset.userToAddId);
    }
}
startConnection();


connection.on('ReceiveMessage', (userId, message) => {
    updateChatList(userId, null, message, false);
});

connection.on('CreateChat', async (userId, chatId, fullName, lastMessage) => {
    try {
        await connection.invoke("LoadChatAsync", chatId.toString());
    }
    catch {
        showToast('Failed to create chat. Please try again later.', colors['error']);
    }

    const noChatsDiv = document.getElementById('no-chats');
    if (noChatsDiv) {
        noChatsDiv.classList.add('newUserAdded');
        document.querySelector('.chat-page-container').classList.add('newUserAdded');
        document.querySelector('.chat-page-container').classList.remove('d-none');
    }
    createChatListItem(userId, chatId, lastMessage, fullName, getCurrentTime());
    const chatIdDiv = document.getElementById('chatId');
    if (chatIdDiv) {
        chatIdDiv.dataset.chatId = chatId;
    }
    makeSoundNotification();
});

function makeSoundNotification() {
    const notificationSound = new Audio('sounds/notificationSound.wav');
    notificationSound.play();
}

async function chooseChat(chatId, userId) {
    const response = await fetch(`Message/Index?chatId=${chatId}&partnerId=${userId}`);
    if (!response.ok) {
        return new Error();
    }
    const html = await response.text();
    const messages = document.getElementById('messages');
    if (messages) {
        messages.innerHTML = html;
        scrollMessageContainer();
    }

    if (selectedChatList) {
        selectedChatList.classList.remove('selected');
    }
    if (noSelectedChatMessage) {
        noSelectedChatMessage.classList.add('d-none');
    }

    changeSidebarVisibility(sidebar, chatPanel, messagePanel, main);
    changeChatItemColor(chatId);
    addMessageSendForEnter();
}

function changeChatItemColor(chatId) {
    const newSelectedChatList = document.querySelector(`.chat-item[data-chat-id="${chatId}"]`);
    newSelectedChatList.classList.add('selected');
    selectedChatList = newSelectedChatList;
}

async function createEmptyChat(userId) {
    try {
        const response = await fetch(`Message/DisplayEmptyChat?userId=${userId}`);
        if (!response.ok) {
            throw new Error();
        }
        const html = await response.text();
        document.getElementById('messages').innerHTML = html;
        addMessageSendForEnter();
    }
    catch {
        showToast('Failed to create new chat. Please try again later.', colors['error']);
    }
}

function addMessageSendForEnter() {
    const textarea = document.querySelector(`textarea[name='text']`);
    textarea.addEventListener('keydown', (e) => {
        if (e.key != 'Enter' || e.shiftKey) {
            return;
        }
        e.preventDefault();
        const form = textarea.closest('form');
        form.dispatchEvent(new Event('submit', {cancelable: true, bubbles: true}))
    });
}

const chatP = document.querySelector('.chat-panel');

if (chatP) {
    chatP.addEventListener('click', async function (e) {
        if (!e.target) {
            return;
        }
        let chatId, userId;
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
        await chooseChat(chatId, userId);
    });
}


async function createMessage(e) {
    e.preventDefault();
    const form = e.target;
    const formData = new FormData(form);
    const csrfToken = document.querySelector(`meta[name='csrf-token']`);
    try {
        const response = await fetch(`Message/CreateMessage`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': csrfToken.content
            },
            body: formData,
        });

        if (!response.ok) {
            throw new Error();
        }

        const chatIdDiv = document.getElementById('chatId');
        let chatId = chatIdDiv.dataset.chatId;
        const partnerId = formData.get('PartnerId');
        const userId = chatLauncher.dataset.yourUserId;
        let fullName = chatLauncher.dataset.yourName;
        const text = formData.get("text");
        if (chatId == '') {
            const result = await response.json();
            chatId = result.chatId;
            fullName = result.fullName;
            chatIdDiv.dataset.chatId = chatId;
            await connection.invoke("CreateChatAsync", userId, partnerId, chatId, fullName, text);
        } else {
            await connection.invoke("SendMessageAsync", chatId.toString(), userId, text);
        }
        updateChatList(partnerId, chatId, formData.get("text"), true, fullName);
        form.reset();
    }
    catch {
        showToast('Failed to create message. Please try again later.', colors['error']);
    }
}

document.addEventListener('submit', (e) => {
    if (e.target && e.target.id) {
        createMessage(e);
    }
});

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

        const chatItem = document.querySelector(`.chat-item[data-user-id='${userId}']`);
        const currentTime = getCurrentTime();
        if (chatItem == null) {
            createChatListItem(userId, chatId, message, userName, currentTime);
            changeChatItemColor(chatId);
        } else {
            const lastMessageText = chatItem.querySelector('.chat-last-message');
            lastMessageText.textContent = message.slice(0, 26);

            const lastMessageDateTime = chatItem.querySelector('.last-message-data-time');

            lastMessageDateTime.textContent = currentTime;
        }

        appendMessage(currentTime, userId, message, isYourMessage);
    }
    catch (err) {
        console.log(err);
    }
}

function escapeHTML(item) {
    const div = document.createElement('div');
    div.textContent = item;
    return div.innerHTML;
}

function createChatListItem(userId, chatId, lastMessageText, fullName, currentTime) {
    const chatPanelDiv = document.querySelector('.chat-panel');

    const chatItemDiv = document.createElement('div');
    chatItemDiv.classList.add('chat-item');
    chatItemDiv.dataset.chatId = escapeHTML(chatId);
    chatItemDiv.dataset.userId = escapeHTML(userId);
    chatPanelDiv.prepend(chatItemDiv);

    const horizontalDiv = document.createElement('div');
    horizontalDiv.classList.add('d-flex');
    chatItemDiv.appendChild(horizontalDiv);

    const nameDiv = document.createElement('div');
    nameDiv.classList.add('chat-title');
    nameDiv.classList.add('overflow-text');
    nameDiv.textContent = fullName;
    horizontalDiv.appendChild(nameDiv);

    const lastMessageDateTimeDiv = document.createElement('div');
    lastMessageDateTimeDiv.classList.add('last-message-data-time');
    lastMessageDateTimeDiv.textContent = currentTime;
    horizontalDiv.appendChild(lastMessageDateTimeDiv);

    const lastMessageTextDiv = document.createElement('div');
    lastMessageTextDiv.classList.add('overflow-text');
    lastMessageTextDiv.classList.add('chat-last-message');
    lastMessageTextDiv.textContent = lastMessageText.substring(0, 26);
    chatItemDiv.appendChild(lastMessageTextDiv);
}

function getCurrentTime() {
    const dateTime = new Date();
    const hours = dateTime.getHours().toString().padStart(2, '0');
    const minutes = dateTime.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
}

function appendMessage(time, userId, message, isYourMessage) {
    const messageContainer = document.querySelector(`.message-container[data-user-id='${userId}']`);
    if (messageContainer) {
        const messageDiv = document.createElement('div');
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
    } else {
        makeSoundNotification();
    }
}