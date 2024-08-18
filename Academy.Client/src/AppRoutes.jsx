// router/index.js
import React from "react";
import { Route, Routes } from "react-router-dom";
import AddStudent from "./components/students/AddStudent";
import EditStudent from "./components/students/EditStudent";
import DeleteStudent from "./components/students/RemoveStudent";
import AddTeacher from "./components/teachers/AddTeacher";
import EditTeacher from "./components/teachers/EditTeacher";
import DeleteTeacher from "./components/teachers/RemoveTeacher";
import AddGroup from "./components/groups/AddGroup";
import EditGroup from "./components/groups/EditGroup";
import DeleteGroup from "./components/groups/DeleteGroup";
import ChangeTeacher from "./components/groups/ChangeTeacher";
import Login from "./components/user/Login";
import Register from "./components/user/Register";
import Logout from "./components/user/Logout";
import Home from "./components/home/Home";
import PrivateRoute from "./components/PrivateRoute";

const AppRouter = ({ isAuthenticated, setIsAuthenticated }) => {
  return (
    <Routes>
      <Route
        path="/login"
        element={<Login setIsAuthenticated={setIsAuthenticated} />}
      />
      <Route path="/register" element={<Register />} />
      <Route
        path="/logout"
        element={<Logout setIsAuthenticated={setIsAuthenticated} />}
      />

      {/* Студенты */}
      <Route
        path="/students/add"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <AddStudent />
          </PrivateRoute>
        }
      />
      <Route
        path="/students/edit"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <EditStudent />
          </PrivateRoute>
        }
      />
      <Route
        path="/students/delete"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <DeleteStudent />
          </PrivateRoute>
        }
      />

      {/* Учителя */}
      <Route
        path="/teachers/add"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <AddTeacher />
          </PrivateRoute>
        }
      />
      <Route
        path="/teachers/edit"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <EditTeacher />
          </PrivateRoute>
        }
      />
      <Route
        path="/teachers/delete"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <DeleteTeacher />
          </PrivateRoute>
        }
      />

      {/* Группы */}
      <Route
        path="/groups/add"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <AddGroup />
          </PrivateRoute>
        }
      />
      <Route
        path="/groups/edit"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <EditGroup />
          </PrivateRoute>
        }
      />
      <Route
        path="/groups/delete"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <DeleteGroup />
          </PrivateRoute>
        }
      />
      <Route
        path="/groups/change-teacher"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <ChangeTeacher />
          </PrivateRoute>
        }
      />

      {/* Пример защищённого маршрута */}
      <Route
        path="/"
        element={
          <PrivateRoute isAuthenticated={isAuthenticated}>
            <Home />
          </PrivateRoute>
        }
      />
      <Route path="*" element={<h1>404 Not Found</h1>} />
    </Routes>
  );
};

export default AppRouter;
