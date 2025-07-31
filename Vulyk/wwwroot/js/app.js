var isOpen = false;
var sidebar;
var chatPanel;
var messagePanel;
var main;
document.addEventListener('DOMContentLoaded', () => {
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
});

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
    if (isOpen == false || window.innerWidth >= 768) {
        return;
    }

    changeSidebarVisibility(sidebar, chatPanel, messagePanel, main);
}