import React from "react";
import { Link } from "react-router-dom";
import { Box, Button, Stack } from "@chakra-ui/react";

const Navbar = ({ isAuthenticated, setIsAuthenticated }) => {
  return (
    <Box bg="gray.800" p={4} color="white">
      <Stack spacing={4} direction="row">
        <Link to="/">Home</Link>
        {isAuthenticated ? (
          <>
            <Link to="/students/add">Add Student</Link>
            <Link to="/teachers/add">Add Teacher</Link>
            <Link to="/groups/add">Add Group</Link>
            <Button
              onClick={() => {
                localStorage.removeItem("token");
                setIsAuthenticated(false);
              }}
              colorScheme="red"
            >
              Logout
            </Button>
          </>
        ) : (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>
        )}
      </Stack>
    </Box>
  );
};

export default Navbar;
