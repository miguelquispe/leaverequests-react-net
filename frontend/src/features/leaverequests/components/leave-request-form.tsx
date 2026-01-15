import { useState } from "react";
import type { LeaveRequestCreate } from "@/core/types/leave-request";

export function LeaveRequestForm() {
  const [formData, setFormData] = useState<LeaveRequestCreate>({
    startDate: "",
    endDate: "",
    reason: "",
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateDates = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.startDate) {
      newErrors.startDate = "Start date is required";
    }

    if (!formData.endDate) {
      newErrors.endDate = "End date is required";
    }

    if (formData.startDate && formData.endDate) {
      const startDate = new Date(formData.startDate);
      const endDate = new Date(formData.endDate);

      if (startDate > endDate) {
        newErrors.startDate = "Start date cannot be after end date";
      }

      if (endDate < startDate) {
        newErrors.endDate = "End date cannot be before start date";
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (validateDates()) {
      // TODO: Add API call
      console.log("Form submitted:", formData);
    }
  };

  const handleInputChange = (
    field: keyof LeaveRequestCreate,
    value: string | number
  ) => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }));

    // Clear error when user starts typing
    if (errors[field]) {
      setErrors((prev) => ({
        ...prev,
        [field]: "",
      }));
    }

    // Re-validate dates when either date changes
    if (field === "startDate" || field === "endDate") {
      setTimeout(() => {
        const updatedFormData = { ...formData, [field]: value };

        if (updatedFormData.startDate && updatedFormData.endDate) {
          const startDate = new Date(updatedFormData.startDate);
          const endDate = new Date(updatedFormData.endDate);

          if (startDate > endDate) {
            setErrors((prev) => ({
              ...prev,
              startDate: "Start date cannot be after end date",
            }));
          } else {
            setErrors((prev) => {
              const newErrors = { ...prev };
              delete newErrors.startDate;
              delete newErrors.endDate;
              return newErrors;
            });
          }
        }
      }, 0);
    }
  };

  return (
    <div className="max-w-md mx-auto bg-white rounded-lg shadow-md p-6">
      <h2 className="text-2xl font-bold text-gray-800 mb-6">
        Create Leave Request
      </h2>

      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Start Date */}
        <div>
          <label
            htmlFor="startDate"
            className="block text-sm font-medium text-gray-700 mb-1"
          >
            Start Date
          </label>
          <input
            type="date"
            id="startDate"
            value={formData.startDate?.split("T")[0] || ""}
            max={formData.endDate?.split("T")[0] || ""}
            onChange={(e) => handleInputChange("startDate", e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          />
          {errors.startDate && (
            <p className="mt-1 text-sm text-red-600">{errors.startDate}</p>
          )}
        </div>

        {/* End Date */}
        <div>
          <label
            htmlFor="endDate"
            className="block text-sm font-medium text-gray-700 mb-1"
          >
            End Date
          </label>
          <input
            type="date"
            id="endDate"
            value={formData.endDate?.split("T")[0] || ""}
            min={formData.startDate?.split("T")[0] || ""}
            onChange={(e) => handleInputChange("endDate", e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          />
          {errors.endDate && (
            <p className="mt-1 text-sm text-red-600">{errors.endDate}</p>
          )}
        </div>

        {/* Reason */}
        <div>
          <label
            htmlFor="reason"
            className="block text-sm font-medium text-gray-700 mb-1"
          >
            Reason
          </label>
          <textarea
            id="reason"
            value={formData.reason || ""}
            onChange={(e) => handleInputChange("reason", e.target.value)}
            rows={3}
            className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 resize-none"
            placeholder="Enter the reason for your leave request..."
          />
          {errors.reason && (
            <p className="mt-1 text-sm text-red-600">{errors.reason}</p>
          )}
        </div>

        {/* Submit Button */}
        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition duration-200 font-medium"
        >
          Submit Leave Request
        </button>
      </form>
    </div>
  );
}
