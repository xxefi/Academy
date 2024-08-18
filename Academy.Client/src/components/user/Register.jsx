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

const Register = () => {
  const { register, handleSubmit, reset } = useForm();
  const toast = useToast();
  const navigate = useNavigate();

  const onSubmit = async (data) => {
    try {
      await axios.post("https://localhost:7259/api/auth/register", data);
      navigate("/login");

      toast({
        title: "Registration successful.",
        description: "You have successfully registered.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      reset();
    } catch (error) {
      toast({
        title: "An error occurred.",
        description: "Unable to register.",
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
            <FormLabel htmlFor="email">Email</FormLabel>
            <Input id="email" placeholder="Email" {...register("email")} />
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
          <FormControl>
            <FormLabel htmlFor="confirmPassword">Confirm Password</FormLabel>
            <Input
              id="confirmPassword"
              type="password"
              placeholder="Confirm Password"
              {...register("confirmPassword")}
            />
          </FormControl>
          <Button type="submit" colorScheme="teal">
            Register
          </Button>
        </Stack>
      </form>
    </Box>
  );
};

export default Register;
