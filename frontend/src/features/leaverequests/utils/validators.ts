import type { LeaveRequestFormData } from "../types/form";

export interface ValidationErrors {
  [key: string]: string;
}

export const validateLeaveRequestForm = (formData: LeaveRequestFormData): ValidationErrors => {
  const errors: ValidationErrors = {};

  if (!formData.startDate) {
    errors.startDate = "Start date is required";
  }

  if (!formData.endDate) {
    errors.endDate = "End date is required";
  }

  if (formData.startDate && formData.endDate) {
    const startDate = new Date(formData.startDate);
    const endDate = new Date(formData.endDate);

    if (startDate > endDate) {
      errors.startDate = "Start date cannot be after end date";
    }

    // Validate that start date is not in the past
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (startDate < today) {
      errors.startDate = "Start date cannot be in the past";
    }
  }

  if (formData.reason?.trim().length === 0) {
    errors.reason = "Reason cannot be empty";
  }
  if (formData.reason && formData.reason.length < 10) {
    errors.reason = "Reason must be at least 10 characters";
  }

  return errors;
};
