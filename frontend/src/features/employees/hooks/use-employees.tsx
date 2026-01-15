import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { employeeApi as employeesApi } from "@/api/employee";
import { mapEmployeeToEmployees } from "../types/employees";
import type { Employee } from "@/core/types/employee";

export const useEmployees = () => {
  const [selectedEmployee, setSelectedEmployee] = useState(() => {
    const saved = localStorage.getItem("selectedEmployee");
    return saved ? JSON.parse(saved) : null;
  });

  const {
    data: employees = [],
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ["employees"],
    queryFn: async () => {
      const response = await employeesApi.getAll();
      if (response.error) throw new Error(response.error);
      return response.data.map(mapEmployeeToEmployees);
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: 1,
  });

  const selectEmployee = (employee: Employee) => {
    setSelectedEmployee(employee);
    localStorage.setItem("selectedEmployee", JSON.stringify(employee));
    localStorage.setItem("userId", employee.id?.toString() || "");
    localStorage.setItem("userRole", employee?.role || "Employee");
  };

  const clearSelection = () => {
    setSelectedEmployee(null);
    localStorage.removeItem("selectedEmployee");
    localStorage.removeItem("userId");
    localStorage.removeItem("userRole");
  };

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
