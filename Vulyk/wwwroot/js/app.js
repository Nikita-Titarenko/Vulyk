let isOpen = false;
let sidebar;
let chatPanel;
let messagePanel;
let main;
const breakpointMd = 768;

sidebar = document.querySelector('.sidebar');
if (sidebar.classList.contains('sidebar-visible')) {
    isOpen = true;
}
chatPanel = document.querySelector('.chat-panel');
messagePanel = document.querySelector('.message-panel');
main = document.querySelector('main');
document.querySelector('.navbar-toggler-custom').addEventListener('click', () => {
    changeSidebarVisibility(sidebar, chatPanel, messagePanel, main);
});

if (messagePanel) {
    messagePanel.addEventListener('click', () => {
        closeSidebar(sidebar, chatPanel, messagePanel, main);
    });
} else {
    main.addEventListener('click', () => {
        closeSidebar(sidebar, chatPanel, messagePanel, main);
    });
}

function changeSidebarVisibility(sidebar, chatPanel, messagePanel, main) {
    isOpen = !isOpen;
    sidebar.classList.toggle('sidebar-invisible');
    sidebar.classList.toggle('sidebar-visible');
    if (messagePanel) {
        messagePanel.classList.toggle('opacity-50');
    } else {
        main.classList.toggle('opacity-50');
    }

    if (chatPanel) {
        chatPanel.classList.toggle('chat-panel-visible');
        chatPanel.classList.toggle('chat-panel-invisible');
    }
}

function closeSidebar(sidebar, chatPanel, messagePanel, main) {
    if (isOpen == false || window.innerWidth >= breakpointMd) {
        return;
    }

    changeSidebarVisibility(sidebar, chatPanel, messagePanel, main);
}
let timer;
function showToast(message, color, duration = null) {
    if (timer) {
        clearTimeout(timer);
    }
    const toast = document.getElementById('toast');
    if (toast) {
        toast.classList.remove('hidden');
        toast.style.background = color;
        const toastMessage = toast.querySelector('#toast-message');
        if (toastMessage) {
            toastMessage.textContent = message;
        }
    }
    if (duration != null) {
        setTimeout(() => {
            toast.classList.add('hidden');
        }, duration);
    }
}

const colors = {
    success: '#5cb85c',
    warning: '#ffcc00',
    error: '#D2042D'
};