import { render, screen } from "@testing-library/react";
import { LeaveRequestFormFields } from "../leave-request-form-fields";
import { describe, test, expect, vi } from "vitest";

describe("LeaveRequestFormFields", () => {
  const mockProps = {
    formData: {
      startDate: "2024-01-01",
      endDate: "2024-01-05",
      reason: "Vacation",
    },
    errors: {},
    onFieldChange: vi.fn(),
    onSubmit: vi.fn(),
    isLoading: false,
  };

  test("renders all form fields correctly", () => {
    render(<LeaveRequestFormFields {...mockProps} />);

    expect(screen.getByLabelText("Start Date")).toBeInTheDocument();
    expect(screen.getByLabelText("End Date")).toBeInTheDocument();
    expect(screen.getByLabelText("Reason")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /submit leave request/i })
    ).toBeInTheDocument();
  });

  test("displays form data correctly", () => {
    render(<LeaveRequestFormFields {...mockProps} />);

    expect(screen.getByDisplayValue("2024-01-01")).toBeInTheDocument();
    expect(screen.getByDisplayValue("2024-01-05")).toBeInTheDocument();
    expect(screen.getByDisplayValue("Vacation")).toBeInTheDocument();
  });

  test("displays validation errors correctly", () => {
    const propsWithErrors = {
      ...mockProps,
      errors: {
        startDate: "Start date is required",
        endDate: "End date must be after start date",
        reason: "Reason must be at least 10 characters",
      },
    };

    render(<LeaveRequestFormFields {...propsWithErrors} />);

    expect(screen.getByText("Start date is required")).toBeInTheDocument();
    expect(
      screen.getByText("End date must be after start date")
    ).toBeInTheDocument();
    expect(
      screen.getByText("Reason must be at least 10 characters")
    ).toBeInTheDocument();

    // Verify error messages have correct styling
    const errorMessages = screen.getAllByText(/required|after|characters/);
    errorMessages.forEach((message) => {
      expect(message).toHaveClass("text-red-600");
    });
  });
});
