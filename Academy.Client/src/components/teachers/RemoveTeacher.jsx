// src/components/teachers/DeleteTeacher.jsx
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

const DeleteTeacher = () => {
  const { register, handleSubmit, reset } = useForm();
  const toast = useToast();

  const onSubmit = async (data) => {
    try {
      await axios.delete(
        `https://localhost:7259/api/teachers/${data.teacherId}`
      );
      toast({
        title: "Teacher deleted.",
        description: "The teacher has been deleted successfully.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      reset();
    } catch (error) {
      toast({
        title: "An error occurred.",
        description: "Unable to delete teacher.",
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
            <FormLabel htmlFor="teacherId">Teacher ID</FormLabel>
            <Input
              id="teacherId"
              placeholder="Teacher ID"
              {...register("teacherId")}
            />
          </FormControl>
          <Button type="submit" colorScheme="red">
            Delete Teacher
          </Button>
        </Stack>
      </form>
    </Box>
  );
};

export default DeleteTeacher;
