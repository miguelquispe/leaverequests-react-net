import { useEmployees } from "../hooks/use-employees";
import { EmployeeSelector } from "../components/employee-selector";

export const EmployeesView = () => {
  const {
    employees,
    selectedEmployee,
    isLoading,
    error,
    selectEmployee,
    clearSelection,
    refetch,
  } = useEmployees();

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">Employees</h1>

      <div className="mb-6">
        <EmployeeSelector
          employees={employees}
          selectedEmployee={selectedEmployee}
          onSelect={selectEmployee}
          onClear={clearSelection}
          isLoading={isLoading}
          error={error}
          onRetry={refetch}
        />
      </div>

      {selectedEmployee && (
        <div className="bg-white border border-gray-200 rounded-lg p-6">
          <h2 className="text-lg font-semibold mb-4">Selected Employee</h2>
          <div className="space-y-2">
            <p>
              <strong>Name:</strong> {selectedEmployee.name}
            </p>
            <p>
              <strong>Email:</strong> {selectedEmployee.email}
            </p>
            <p>
              <strong>Role:</strong> {selectedEmployee.role}
            </p>
            <p>
              <strong>ID:</strong> {selectedEmployee.id}
            </p>
          </div>
        </div>
      )}
    </div>
  );
};
