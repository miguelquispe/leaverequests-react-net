import type { LeaveRequest } from "@/core/types/leave-request";
import apiClient from "./api-client";

// Local aliases for clarity
type LeaveRequestDTO = LeaveRequest;

export const leaveRequestApi = {
  getAll: () => apiClient.get<LeaveRequestDTO[]>("/leaverequests"),
  getById: (id: string) => apiClient.get<LeaveRequestDTO>(`/leaverequests/${id}`),
  create: (data: LeaveRequestDTO) => apiClient.post("/leaverequests", data),
  update: (id: string, data: LeaveRequestDTO) => apiClient.put(`/leaverequests/${id}`, data),
  delete: (id: string) => apiClient.delete(`/leaverequests/${id}`),
}