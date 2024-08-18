import { Box, Button, FormControl, Stack, useToast } from "@chakra-ui/react";
import axios from "axios";
import { useForm } from "react-hook-form";

export default function AddStudent() {
  const [register, handleSubmit, reset] = useForm();
  const toast = useToast();

  const onSubmit = async (data) => {
    try {
      await axios.post("https://localhost:7259/api/students", data);
      toast({
        title: "Student added",
        description: "The student has been added successfully",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      reset();
    } catch (e) {
      toast({
        title: "An error occurred.",
        description: "Unable to add student.",
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
          <FormControl>
            <FormLabel htmlFor="departmentId">Department ID:</FormLabel>
            <Input
              id="departmentId"
              placeholder="Department ID..."
              {...register("departmentId")}
            />
          </FormControl>
          <Button type="submit" colorScheme="teal">
            Add student
          </Button>
        </Stack>
      </form>
    </Box>
  );
}
