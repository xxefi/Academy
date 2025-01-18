<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Role Management Dashboard</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/font-awesome/css/font-awesome.min.css" rel="stylesheet">
    <link rel="stylesheet" href="index.css">
</head>
<body>
    <header class="text-center">
        <h1>Role Management Dashboard</h1>
    </header>

    <nav class="navbar navbar-expand-lg navbar-dark">
        <ul class="navbar-nav ms-auto">
            <li class="nav-item"><a class="nav-link" href="#get-roles"><i class="fa fa-list"></i> Get All Roles</a></li>
            <li class="nav-item"><a class="nav-link" href="#add-role"><i class="fa fa-plus-circle"></i> Add Role</a></li>
            <li class="nav-item"><a class="nav-link" href="#update-role"><i class="fa fa-pencil"></i> Update Role</a></li>
            <li class="nav-item"><a class="nav-link" href="#delete-role"><i class="fa fa-trash"></i> Delete Role</a></li>
            <li class="nav-item"><a class="nav-link" href="#role-users"><i class="fa fa-users"></i> Role Users</a></li>
            <li class="nav-item"><a class="nav-link" href="#role-existence"><i class="fa fa-search"></i> Role Existence</a></li>
        </ul>
    </nav>

    <div id="role-modal" class="modal fade" tabindex="-1" aria-labelledby="role-modal-label" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="role-modal-label">Role Details</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <p><strong>ID:</strong> <span id="modal-role-id"></span></p>
                        <p><strong>Name:</strong> <span id="modal-role-name"></span></p>
                        <p><strong>Description:</strong> <span id="modal-role-description"></span></p>
                        <p><strong>User IDs:</strong> <span id="modal-role-usersid"></span></p>
    </div>

                </div>
            </div>
        </div>

    <div class="container">
        <section id="get-roles">
            <h2 class="section-title">Get All Roles</h2>
            <button class="btn btn-custom btn-primary" onclick="fetchRoles()">Fetch Roles</button>
            <button class="btn btn-custom btn-danger" onclick="removeRoles()">REMOVE ALL ROLES</button>
            <table class="table table-striped mt-3" id="roles-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Description</th>
                    </tr>
                </thead>
                <tbody id="roles-list">
                </tbody>
            </table>
        </section>
        <section id="add-role">
            <h2 class="section-title">Add a New Role</h2>
            <form id="add-role-form">
                <div class="mb-3">
                    <input type="text" name="roleName" class="form-control" placeholder="Role Name" required>
                </div>
                <div class="mb-3">
                    <input type="text" name="roleDescription" class="form-control" placeholder="Role Description">
                </div>
                <button type="submit" class="btn btn-custom btn-success">Add Role</button>
            </form>
        </section>
        <section id="update-role">
            <h2 class="section-title">Update Role</h2>
            <form id="update-role-form">
                <div class="mb-3">
                    <input type="text" name="roleId" class="form-control" placeholder="Role ID" required>
                </div>
                <div class="mb-3">
                    <input type="text" name="roleName" class="form-control" placeholder="New Role Name" required>
                </div>
                <button type="submit" class="btn btn-custom btn-warning">Update Role</button>
            </form>
        </section>
        <section id="delete-role">
            <h2 class="section-title">Delete Role</h2>
            <form id="delete-role-form">
                <div class="mb-3">
                    <input type="text" name="roleId" class="form-control" placeholder="Role ID" required>
                </div>
                <button type="submit" class="btn btn-custom btn-danger">Delete Role</button>
            </form>
        </section>
        <section id="role-users">
            <h2 class="section-title">Role Users Management</h2>
            <form id="role-users-form">
                <div class="mb-3">
                    <input type="text" name="roleId" class="form-control" placeholder="Role ID" required>
                </div>
                <button type="button" class="btn btn-custom btn-primary" onclick="fetchRoleUsers()">Get Users</button>
            </form>
            <ul id="role-users-list" class="list-group mt-3"></ul>
        </section>
        <section id="role-existence">
            <h2 class="section-title">Check Role Existence</h2>
            <form id="check-role-form">
                <div class="mb-3">
                    <input type="text" name="roleId" class="form-control" placeholder="Role ID">
                </div>
                <button type="button" class="btn btn-custom btn-info" onclick="checkRoleExistence()">Check</button>
            </form>
            <p id="role-existence-result" class="mt-3"></p>
        </section>
    </div>

    <div class="toast-container position-fixed top-0 end-0 p-3" id="toast-container"></div>

    <footer>
        <p style="display: flex;justify-content: center;">&copy; 2025 Role Management. All rights reserved.</p>
    </footer>

    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.11.6/dist/umd/popper.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/js/bootstrap.bundle.min.js"></script>
    <script src="index.js"></script>
</body>
</html>
