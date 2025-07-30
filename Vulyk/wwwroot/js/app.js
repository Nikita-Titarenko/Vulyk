var isOpen = false;

document.addEventListener('DOMContentLoaded', () => {
    const sidebar = document.querySelector('.sidebar-invisible');
    const chatPanel = document.querySelector('.chat-panel');
    const messagePanel = document.querySelector('.message-panel');
    const main = document.querySelector('main');
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