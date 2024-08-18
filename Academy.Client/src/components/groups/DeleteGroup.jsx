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

const DeleteGroup = () => {
  const { register, handleSubmit, reset } = useForm();
  const toast = useToast();

  const onSubmit = async (data) => {
    try {
      await axios.delete(`/api/groups/${data.groupId}`);
      toast({
        title: "Group deleted.",
        description: "The group has been deleted successfully.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      reset();
    } catch (error) {
      toast({
        title: "An error occurred.",
        description: "Unable to delete group.",
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
            <FormLabel htmlFor="groupId">Group ID</FormLabel>
            <Input
              id="groupId"
              placeholder="Group ID"
              {...register("groupId")}
            />
          </FormControl>
          <Button type="submit" colorScheme="red">
            Delete Group
          </Button>
        </Stack>
      </form>
    </Box>
  );
};

export default DeleteGroup;
