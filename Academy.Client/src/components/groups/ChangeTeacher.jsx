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

const ChangeTeacher = () => {
  const { register, handleSubmit, reset } = useForm();
  const toast = useToast();

  const onSubmit = async (data) => {
    try {
      await axios.put(`/api/groups/change-teacher`, data);
      toast({
        title: "Teacher changed.",
        description: "The teacher has been changed successfully.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      reset();
    } catch (error) {
      toast({
        title: "An error occurred.",
        description: "Unable to change teacher.",
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
            <FormLabel htmlFor="currentTeacherId">Current Teacher ID</FormLabel>
            <Input
              id="currentTeacherId"
              placeholder="Current Teacher ID"
              {...register("currentTeacherId")}
            />
          </FormControl>
          <FormControl>
            <FormLabel htmlFor="newTeacherId">New Teacher ID</FormLabel>
            <Input
              id="newTeacherId"
              placeholder="New Teacher ID"
              {...register("newTeacherId")}
            />
          </FormControl>
          <FormControl>
            <FormLabel htmlFor="groupId">Group ID</FormLabel>
            <Input
              id="groupId"
              placeholder="Group ID"
              {...register("groupId")}
            />
          </FormControl>
          <Button type="submit" colorScheme="teal">
            Change Teacher
          </Button>
        </Stack>
      </form>
    </Box>
  );
};

export default ChangeTeacher;
