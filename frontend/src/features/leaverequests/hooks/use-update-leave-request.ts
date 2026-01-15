import { useMutation, useQueryClient } from '@tanstack/react-query';
import { leaveRequestApi } from '@/api/leave-request';
import type { LeaveRequest } from '@/core/types/leave-request';

type UpdateLeaveRequestParams = {
  id: string;
  data: LeaveRequest;
};

export function useUpdateLeaveRequest() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: UpdateLeaveRequestParams) => {
      return leaveRequestApi.update(id, data);
    },
    onSuccess: () => {
      // Invalidate and refetch leave requests
      queryClient.invalidateQueries({ queryKey: ['leaveRequests'] });
    },
    onError: (error) => {
      console.error('Failed to update leave request:', error);
    },
  });
}