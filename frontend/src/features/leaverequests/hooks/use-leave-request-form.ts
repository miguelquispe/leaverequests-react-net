import { useState } from "react";
import {
    validateLeaveRequestForm,
    type ValidationErrors,
} from "../utils/validators";
import { useCreateLeaveRequest } from "./use-create-leave-request";
import { useAuth } from "@/contexts/AuthContext";
import type { LeaveRequestFormData } from "../types/form";

interface UseLeaveRequestFormProps {
  onSuccess?: (message: string) => void;
  onError?: (error: { message: string; errors: string[] }) => void;
}


export function useLeaveRequestForm({
  onSuccess,
  onError,
}: UseLeaveRequestFormProps = {}) {
  const { user } = useAuth();
  const [formData, setFormData] = useState<LeaveRequestFormData>({
    startDate: "",
    endDate: "",
    reason: "",
  });

  const [errors, setErrors] = useState<ValidationErrors>({});
  const createMutation = useCreateLeaveRequest();
  const updateField = (
    field: keyof LeaveRequestFormData,
    value: string
  ) => {
    const updatedData = { ...formData, [field]: value };
    setFormData(updatedData);

    // Clear field error when user starts typing
    if (errors[field]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }

    // Real-time validation for dates
    if (field === "startDate" || field === "endDate") {
      setTimeout(() => {
        const fieldErrors = validateLeaveRequestForm(updatedData);
        setErrors((prev) => ({
          ...prev,
          ...fieldErrors,
        }));
      }, 0);
    }
  };

  const validateForm = (): boolean => {
    const newErrors = validateLeaveRequestForm(formData);
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const resetForm = () => {
    setFormData({
      startDate: "",
      endDate: "",
      reason: "",
    });
    setErrors({});
  };

  const submitForm = async () => {
    // Ensure we have a valid employeeId
    if (!user?.id) {
      onError?.({
        message: "Authentication error. Please log in again.",
        errors: [],
      });
      return;
    }

    // Update formData with current user ID just before submission
    const submitData = { ...formData, employeeId: user.id };

    if (!validateForm()) {
      return;
    }

    try {
      await createMutation.mutateAsync(submitData);
      onSuccess?.("Leave request created successfully!");
      resetForm();
    } catch (error) {
      console.error("Error creating leave request:", error);
      let message = "Failed to create leave request. Please try again.";
      let errorsDetails: string[] = [];
      // Check if it's an API error with the expected structure
      if (error && typeof error === "object" && "response" in error) {
        const response = error.response;
        //   const data = response?.data || {};
        if (response?.data?.errors) {
          const errorData = response.data.errors;

          console.log("xxxx", errorData);

          switch (errorData.code) {
            case "VALIDATION_ERROR":
              //       // For validation errors, use details if available
              //   message = errorData.details || errorData.message ||
              message = "Validation failed. Please check your input.";
              errorsDetails = Object.values(
                errorData.details
              ).flat() as string[];
              break;
            case "OVERLAPPING_REQUEST":
              // For overlapping requests, use the direct message
              message =
                errorData.message ||
                "You have an overlapping leave request for the selected dates.";
              break;
            default:
              // Fallback for other error codes
              message =
                errorData.message ||
                "An error occurred while processing your request.";
          }
        } else if (response?.data?.message) {
          message = response.data.message;
        } else if (error instanceof Error) {
          message = error.message;
        }
      }

      onError?.({ message, errors: errorsDetails });
    }
  };

  return {
    formData,
    errors,
    updateField,
    submitForm,
    resetForm,
    isLoading: createMutation.isPending,
    isValid: Object.keys(errors).length === 0,
  };
}
