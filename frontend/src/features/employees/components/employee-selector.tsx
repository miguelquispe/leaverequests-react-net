import type { Employee } from "@/core/types/employee";
import { memo } from "react";

interface EmployeeSelectorProps {
  employees: Employee[];
  selectedEmployee: Employee | null;
  onSelect: (employee: Employee) => void;
  onClear: () => void;
  isLoading: boolean;
  error: string | null;
  onRetry: () => void;
}

export const EmployeeSelector = memo(
  ({
    employees,
    selectedEmployee,
    onSelect,
    onClear,
    isLoading,
    error,
    onRetry,
  }: EmployeeSelectorProps) => {
    const handleEmployeeChange = (
      event: React.ChangeEvent<HTMLSelectElement>
    ) => {
      const employeeId = parseInt(event.target.value);
      const employee = employees.find((emp) => emp.id === employeeId);

      if (employee) {
        onSelect(employee);
      } else {
        onClear();
      }
    };

    if (error) {
      return (
        <div className="p-3 bg-red-50 border border-red-200 rounded-md">
          <p className="text-red-700 text-sm mb-2">{error}</p>
          <button
            onClick={onRetry}
            className="text-red-600 hover:text-red-800 text-sm underline"
          >
            Retry
          </button>
        </div>
      );
    }

    if (isLoading)
      return <div className="text-gray-500">Loading employees...</div>;

    return (
      <div>
        <label className="block text-sm font-medium mb-2">Employee:</label>
        <select
          value={selectedEmployee?.id || ""}
          onChange={handleEmployeeChange}
          className="w-full p-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="">Select employee...</option>
          {employees.map((emp) => (
            <option key={emp.id} value={emp.id}>
              {emp.name} ({emp.role}) - {emp.email}
            </option>
          ))}
        </select>
      </div>
    );
  }
);
