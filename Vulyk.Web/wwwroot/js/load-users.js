let page = 1;
let pageSize;
let rowHeight;
const table = document.querySelector('tbody');;
const tableContainer = document.querySelector('.table-container');
let isLoading = false;

async function loadUsers() {
    try {
        const response = await fetch(`/Admin/User/LoadUsers?page=${++page}`);
        const result = await response.json();
        if (!result.isSuccess) {
            return;
        }
        for (let user of result.value.users) {
            const tr = document.createElement('tr');
            tr.innerHTML = `
				<td>
					${user.fullName}
				</td>
				<td>
					${user.email}
				</td>
				<td>
					${user.status}
				</td>
				<td>
					${user.role}
				</td>
                `;
            table.appendChild(tr);
        }
    }
    catch {
        showToast('Failed to load users. Please try again later.', colors['error']);
    }
    finally {
        isLoading = false;
    }
}

const div = document.querySelector('[data-page-size]');
if (div) {
    pageSize = parseInt(div.dataset.pageSize);
    const td = document.querySelector('td');

    if (td) {
        rowHeight = td.offsetHeight;
    }
} else {
    pageSize = 20;
}


tableContainer.addEventListener('scroll', () => {
    if (!isLoading && tableContainer.scrollHeight * 0.8 <= tableContainer.scrollTop + tableContainer.clientHeight) {
        isLoading = true;
        loadUsers();
    }
});