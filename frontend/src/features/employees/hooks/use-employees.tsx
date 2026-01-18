import { useQuery } from "@tanstack/react-query";
import { useState, useCallback } from "react";
import { employeeApi as employeesApi } from "@/api/employee";
import { mapEmployeeToEmployees } from "../types/employees";
import type { Employee } from "@/core/types/employee";

export const useEmployees = () => {
  const [selectedEmployee, setSelectedEmployee] = useState<Employee | null>(
    null
  );

  const {
    data: employees = [],
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ["employees"],
    queryFn: async () => {
      const response = await employeesApi.getAll();

      return (response.data || ([] as Employee[])).map(mapEmployeeToEmployees);
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: 1,
  });

  const selectEmployee = useCallback((employee: Employee) => {
    setSelectedEmployee(employee);
    localStorage.setItem("userId", employee.id?.toString() || "");
    localStorage.setItem("userRole", employee?.role || "Employee");
  }, []);

  const clearSelection = useCallback(() => {
    setSelectedEmployee(null);
    localStorage.removeItem("userId");
    localStorage.removeItem("userRole");
  }, []);

  return {
    employees,
    selectedEmployee,
    isLoading,
    error: error?.message || null,
    selectEmployee,
    clearSelection,
    refetch,
  };
};
