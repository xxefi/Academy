const API_BASE_URL =
  "https://academy-a7c3gcbcdbareagk.canadacentral-01.azurewebsites.net/api/Roles";

function showToast(message, type = "success") {
  const toastContainer = document.getElementById("toast-container");

  const toastId = `toast-${Date.now()}`;
  const toastHTML = `
    <div id="${toastId}" class="toast align-items-center text-bg-${type} border-0" role="alert" aria-live="assertive" aria-atomic="true">
      <div class="d-flex">
        <div class="toast-body">
          ${message}
        </div>
        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
      </div>
    </div>
  `;

  toastContainer.insertAdjacentHTML("beforeend", toastHTML);

  const toastElement = document.getElementById(toastId);
  const bootstrapToast = new bootstrap.Toast(toastElement);
  bootstrapToast.show();

  toastElement.addEventListener("hidden.bs.toast", () => {
    toastElement.remove();
  });
}

async function fetchRoles() {
  try {
    const response = await fetch(API_BASE_URL + "/GetRoles");
    if (!response.ok) throw new Error("Failed to fetch roles");

    const data = await response.json();

    if (!Array.isArray(data.data)) {
      throw new Error("The fetched roles are not an array");
    }

    const roles = data.data;

    const rolesList = document.getElementById("roles-list");
    rolesList.innerHTML = "";

    roles.forEach((role) => {
      const row = document.createElement("tr");

      const idCell = document.createElement("th");
      const idLink = document.createElement("a");
      idLink.style.textDecoration = "none";
      idLink.href = `#`;
      idLink.textContent = role.id;
      idLink.onclick = (event) => {
        event.preventDefault();
        showRoleModal(role.id);
      };
      idCell.appendChild(idLink);

      const nameCell = document.createElement("td");
      const nameCell2 = document.createElement("strong");
      nameCell2.textContent = role.name;
      nameCell.appendChild(nameCell2);

      const descriptionCell = document.createElement("td");
      const descriptionCell2 = document.createElement("strong");
      descriptionCell2.textContent = role.description || "-";
      descriptionCell.appendChild(descriptionCell2);

      row.appendChild(idCell);
      row.appendChild(nameCell);
      row.appendChild(descriptionCell);

      rolesList.appendChild(row);
    });

    showToast("Roles fetched successfully!", "success");
  } catch (error) {
    showToast(error.message, "danger");
  }
}

async function removeRoles() {
  const confirmation = confirm("HEYQI SILMEK ISTISEN HERSEYI???");
  if (!confirmation) return;
  try {
    const response = await fetch(API_BASE_URL + "/Delete/All", {
      method: "DELETE",
    });
    response.json();
    showToast("All roles deleted successfully", "success");
    fetchRoles();
  } catch (error) {
    showToast(error.message, "danger");
  }
}
document
  .getElementById("add-role-form")
  .addEventListener("submit", async (e) => {
    e.preventDefault();
    const roleName = e.target.roleName.value;
    const roleDescription = e.target.roleDescription.value;
    try {
      const response = await fetch(API_BASE_URL + "/CreateRole", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: roleName,
          description: roleDescription,
        }),
      });

      if (!response.ok) {
        const errorResponse = await response.json();
        if (errorResponse.type === "CredentialsAlreadyExists") {
          showToast(errorResponse.message, "danger");
        } else {
          showToast("Failed to add role: " + errorResponse.message, "danger");
        }
        return;
      }
      fetchRoles();
    } catch (error) {
      showToast("An unexpected error occurred: " + error.message, "danger");
    }
  });

document
  .getElementById("update-role-form")
  .addEventListener("submit", async (e) => {
    e.preventDefault();
    const roleId = e.target.roleId.value;
    const roleName = e.target.roleName.value;
    try {
      const response = await fetch(`${API_BASE_URL}/Update/ID/${roleId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name: roleName }),
      });

      if (!response.ok) {
        const errorResponse = await response.json();
        if (errorResponse.type === "CredentialsAlreadyExists") {
          showToast("Role with this name already exists", "error");
        } else {
          showToast("Failed to update role: " + errorResponse.message, "error");
        }
        return;
      }

      showToast("Role updated successfully", "success");
      fetchRoles();
    } catch (error) {
      showToast("An unexpected error occurred: " + error.message, "error");
    }
  });

document
  .getElementById("delete-role-form")
  .addEventListener("submit", async (e) => {
    e.preventDefault();
    const roleId = e.target.roleId.value;
    try {
      const response = await fetch(`${API_BASE_URL}/Delete/ID/${roleId}`, {
        method: "DELETE",
      });

      if (!response.ok) {
        const errorResponse = await response.json();
        showToast("Failed to delete role: " + errorResponse.message, "error");
        return;
      }

      showToast("Role deleted successfully", "success");
      fetchRoles();
    } catch (error) {
      showToast("An unexpected error occurred: " + error.message, "error");
    }
  });

async function fetchRoleUsers() {
  const roleId = document.getElementById("role-users-form").roleId.value;
  try {
    const response = await fetch(`${API_BASE_URL}/${roleId}/users`);
    if (!response.ok) throw new Error("Failed to fetch role users");
    const users = await response.json();
    const usersList = document.getElementById("role-users-list");
    usersList.innerHTML = "";
    users.forEach((user) => {
      const li = document.createElement("li");
      li.textContent = user.name;
      usersList.appendChild(li);
    });
  } catch (error) {
    showToast(error.message, "error");
  }
}

async function checkRoleExistence() {
  const roleId = document.getElementById("check-role-form").roleId.value;
  try {
    const response = await fetch(`${API_BASE_URL}/exists/id/${roleId}`);
    if (!response.ok) throw new Error("Failed to check role existence");
    const result = await response.json();
    document.getElementById("role-existence-result")
      ? (textContent = result)
      : (textContent = "false");
  } catch (error) {
    showToast(error.message, "error");
  }
}

async function showRoleModal(roleId) {
  try {
    const response = await fetch(`${API_BASE_URL}/ID/${roleId}`);
    if (!response.ok) throw new Error("Failed to fetch role details");
    const role = await response.json();

    console.log(role);

    document.getElementById("modal-role-id").textContent = role.data.id;
    document.getElementById("modal-role-name").textContent = role.data.name;
    document.getElementById("modal-role-description").textContent =
      role.data.description || "---";

    const modalUsersList = document.getElementById("modal-role-usersid");
    modalUsersList.innerHTML = ""; // Очищаем текущий контент

    if (role.data.userIds && role.data.userIds.length > 0) {
      const ul = document.createElement("ol");

      role.data.userIds.forEach((userId) => {
        const li = document.createElement("li");
        li.textContent = userId;
        ul.appendChild(li);
      });

      modalUsersList.appendChild(ul);
    } else {
      modalUsersList.textContent = "---";
    }

    const modal = new bootstrap.Modal(document.getElementById("role-modal"));
    modal.show();
  } catch (error) {
    showToast(error.message, "error");
  }
}

function closeModal() {
  const modal = document.getElementById("role-modal");
  modal.style.display = "none";
}

window.onclick = function (event) {
  const modal = document.getElementById("role-modal");
  if (event.target === modal) {
    modal.style.display = "none";
  }
};
