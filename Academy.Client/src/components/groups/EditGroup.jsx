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

const EditGroup = ({ group }) => {
  const { register, handleSubmit, reset } = useForm({
    defaultValues: group,
  });
  const toast = useToast();

  const onSubmit = async (data) => {
    try {
      await axios.put(`/api/groups/${group.id}`, data);
      toast({
        title: "Group updated.",
        description: "The group details have been updated successfully.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      reset();
    } catch (error) {
      toast({
        title: "An error occurred.",
        description: "Unable to update group.",
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
            <FormLabel htmlFor="name">Group Name</FormLabel>
            <Input id="name" placeholder="Group Name" {...register("name")} />
          </FormControl>
          <Button type="submit" colorScheme="teal">
            Update Group
          </Button>
        </Stack>
      </form>
    </Box>
  );
};

export default EditGroup;
