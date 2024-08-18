import React from "react";
import {
  Box,
  Button,
  FormControl,
  FormLabel,
  Input,
  Stack,
  useToast,
} from "@chakra-ui/react";
import { useForm } from "react-hook-form";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const Login = ({ setIsAuthenticated }) => {
  const { register, handleSubmit, reset } = useForm();
  const toast = useToast();
  const navigate = useNavigate();

  const onSubmit = async (data) => {
    try {
      const response = await axios.post(
        "https://localhost:7259/api/auth/login",
        data
      );
      localStorage.setItem("token", response.data.token);
      setIsAuthenticated(true);
      navigate("/");

      toast({
        title: "Login successful.",
        description: "You have successfully logged in.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      reset();
    } catch (error) {
      toast({
        title: "An error occurred.",
        description: "Unable to log in.",
        status: "error",
        duration: 5000,
        isClosable: true,
      });
    }
  };

  return (
    <Box
      p={4}
      maxW="md"
      mx="auto"
      mt={5}
      borderWidth={1}
      borderRadius="md"
      boxShadow="lg"
    >
      <form onSubmit={handleSubmit(onSubmit)}>
        <Stack spacing={4}>
          <FormControl>
            <FormLabel htmlFor="username">Username</FormLabel>
            <Input
              id="username"
              placeholder="Username"
              {...register("username")}
            />
          </FormControl>
          <FormControl>
            <FormLabel htmlFor="password">Password</FormLabel>
            <Input
              id="password"
              type="password"
              placeholder="Password"
              {...register("password")}
            />
          </FormControl>
          <Button type="submit" colorScheme="teal">
            Login
          </Button>
        </Stack>
      </form>
    </Box>
  );
};

export default Login;
