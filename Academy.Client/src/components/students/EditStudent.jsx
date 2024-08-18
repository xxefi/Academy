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

const EditStudent = ({ student }) => {
  const { register, handleSubmit, reset } = useForm({
    defaultValues: student,
  });
  const toast = useToast();

  const onSubmit = async (data) => {
    try {
      await axios.put(
        `https://localhost:7259/api/students/${student.id}`,
        data
      );
      toast({
        title: "Student updated.",
        description: "The student details have been updated successfully.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      reset();
    } catch (error) {
      toast({
        title: "An error occurred.",
        description: "Unable to update student.",
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
            <FormLabel htmlFor="firstName">First Name</FormLabel>
            <Input
              id="firstName"
              placeholder="First Name"
              {...register("firstName")}
            />
          </FormControl>
          <FormControl>
            <FormLabel htmlFor="lastName">Last Name</FormLabel>
            <Input
              id="lastName"
              placeholder="Last Name"
              {...register("lastName")}
            />
          </FormControl>
          <Button type="submit" colorScheme="teal">
            Update Student
          </Button>
        </Stack>
      </form>
    </Box>
  );
};

export default EditStudent;
