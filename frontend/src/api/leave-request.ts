import type { LeaveRequest } from "@/core/types/leave-request";
import apiClient, { apiResponse } from "./api-client";

// Local aliases for clarity
type LeaveRequestDTO = LeaveRequest;

export const leaveRequestApi = {
  getAll: () => apiResponse<LeaveRequestDTO[]>(apiClient.get("/leaverequests")),
  getById: (id: string) => apiResponse<LeaveRequestDTO>(apiClient.get(`/leaverequests/${id}`)),
  create: (data: LeaveRequestDTO) => apiResponse<LeaveRequestDTO>(apiClient.post("/leaverequests", data)),
  update: (id: string, data: LeaveRequestDTO) => apiResponse<LeaveRequestDTO>(apiClient.put(`/leaverequests/${id}`, data)),
  delete: (id: string) => apiResponse<void>(apiClient.delete(`/leaverequests/${id}`)),
}