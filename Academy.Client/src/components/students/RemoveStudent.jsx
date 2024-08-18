// src/components/students/DeleteStudent.jsx
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

const DeleteStudent = () => {
  const { register, handleSubmit, reset } = useForm();
  const toast = useToast();

  const onSubmit = async (data) => {
    try {
      await axios.delete(
        `https://localhost:7259/api/students/${data.studentId}`
      );
      toast({
        title: "Student deleted.",
        description: "The student has been deleted successfully.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      reset();
    } catch (error) {
      toast({
        title: "An error occurred.",
        description: "Unable to delete student.",
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
            <FormLabel htmlFor="studentId">Student ID</FormLabel>
            <Input
              id="studentId"
              placeholder="Student ID"
              {...register("studentId")}
            />
          </FormControl>
          <Button type="submit" colorScheme="red">
            Delete Student
          </Button>
        </Stack>
      </form>
    </Box>
  );
};

export default DeleteStudent;
