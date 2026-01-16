import type { Employee } from "@/core/types/employee";
import apiClient, { apiResponse } from "./api-client";

// Local aliases for clarity
type EmployeeDTO = Employee;

export const employeeApi = {
  getAll: async () => apiResponse<EmployeeDTO[]>(apiClient.get("/employees"))
};
