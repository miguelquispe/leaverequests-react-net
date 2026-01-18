import { useEmployees } from "../hooks/use-employees";
import { EmployeeSelector } from "../components/employee-selector";
import type { Employee } from "@/core/types/employee";

export const EmployeesView = ({
  onEmployeeSelect,
}: {
  onEmployeeSelect: (employee: Employee) => void;
}) => {
  const {
    employees,
    selectedEmployee,
    isLoading,
    error,
    selectEmployee,
    clearSelection,
    refetch,
  } = useEmployees();

  const handleSelect = (employee: Employee) => {
    selectEmployee(employee);
    onEmployeeSelect(employee);
  };

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">Select Employee</h1>

      <div className="mb-6">
        <EmployeeSelector
          employees={employees}
          selectedEmployee={selectedEmployee}
          onSelect={handleSelect}
          onClear={clearSelection}
          isLoading={isLoading}
          error={error}
          onRetry={refetch}
        />
      </div>
    </div>
  );
};
