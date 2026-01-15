import type { Employee } from "@/core/types/leave-request";
import apiClient from "./api-client";

// Local aliases for clarity
type EmployeeDTO = Employee;

export const employeeApi = {
  getAll: () => apiClient.get<EmployeeDTO[]>("/employees"),
}