import type { LeaveRequestCreate } from "@/core/types/leave-request";
import type { ValidationErrors } from "../utils/validators";

interface LeaveRequestFormFieldsProps {
  formData: LeaveRequestCreate;
  errors: ValidationErrors;
  onFieldChange: (field: keyof LeaveRequestCreate, value: string) => void;
  onSubmit: (e: React.FormEvent) => void;
  isLoading?: boolean;
}

export function LeaveRequestFormFields({
  formData,
  errors,
  onFieldChange,
  onSubmit,
  isLoading = false,
}: LeaveRequestFormFieldsProps) {
  return (
    <form onSubmit={onSubmit} className="space-y-4">
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
          onChange={(e) => onFieldChange("startDate", e.target.value)}
          disabled={isLoading}
          className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-gray-100"
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
          onChange={(e) => onFieldChange("endDate", e.target.value)}
          disabled={isLoading}
          className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-gray-100"
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
          maxLength={100}
          value={formData.reason || ""}
          onChange={(e) => onFieldChange("reason", e.target.value)}
          rows={3}
          disabled={isLoading}
          className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 resize-none disabled:bg-gray-100"
          placeholder="Enter the reason for your leave request..."
        />
        {errors.reason && (
          <p className="mt-1 text-sm text-red-600">{errors.reason}</p>
        )}
      </div>

      {/* Submit Button */}
      <button
        type="submit"
        disabled={isLoading}
        className="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition duration-200 font-medium disabled:bg-blue-400 disabled:cursor-not-allowed"
      >
        {isLoading ? "Creating..." : "Submit Leave Request"}
      </button>
    </form>
  );
}
