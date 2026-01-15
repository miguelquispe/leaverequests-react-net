import { useMutation, useQueryClient } from '@tanstack/react-query';
import { leaveRequestApi } from '@/api/leave-request';
import type { LeaveRequestCreate } from '@/core/types/leave-request';

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
    onError: (error) => {
      console.error('Failed to create leave request:', error);
    },
  });
}