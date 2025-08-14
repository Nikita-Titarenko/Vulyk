var page = 1;
var pageSize;
var rowHeight;
var table;
var tableContainer;
var isLoading = false;

async function loadUsers() {
    const response = await fetch(`/Admin/User/LoadUsers?page=${++page}`);
    const result = await response.json();
    if (!result.isSuccess) {
        return;
    }
    for (var user of result.value.users) {
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

document.addEventListener('DOMContentLoaded', () => {
    var div = document.querySelector('[data-page-size]');
    if (div) {
        pageSize = parseInt(div.dataset.pageSize);
        var td = document.querySelector('td');

        if (td) {
            rowHeight = td.offsetHeight;
        }
    } else {
        pageSize = 20;
    }
    table = document.querySelector('tbody');
    tableContainer = document.querySelector('.table-container');
    tableContainer.addEventListener('scroll', () => {
        if (!isLoading && tableContainer.scrollHeight * 0.8 <= tableContainer.scrollTop + tableContainer.clientHeight) {
            isLoading = true;
            loadUsers();
            isLoading = false;
        }
    });
});