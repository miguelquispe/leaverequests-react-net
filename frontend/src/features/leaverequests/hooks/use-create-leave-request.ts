import { useMutation, useQueryClient } from '@tanstack/react-query';
import { leaveRequestApi } from '@/api/leave-request';
import type { LeaveRequestCreate } from '@/core/types/leave-request';
import type { AxiosError } from 'axios';
import type { ApiErrorResponse } from '@/core/types/api-response';

export function useCreateLeaveRequest() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: LeaveRequestCreate) => {
      return leaveRequestApi.create(data);
    },
    onSuccess: () => {
      // Invalidate and refetch leave requests
      queryClient.invalidateQueries({ queryKey: ['leaveRequests'] });
    },
    onError: (error: AxiosError<ApiErrorResponse>) => {
      console.error('Failed to create leave request:', error.response?.data);
      
      // Extract business error message if available
      if (error.response?.data.errors) {
        console.error('Business Error:', error.response.data);
        // Don't throw here - let the mutation handle the error naturally
        return;
      }
    },
  });
}