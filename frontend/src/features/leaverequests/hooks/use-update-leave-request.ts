import { useMutation, useQueryClient } from '@tanstack/react-query';
import { leaveRequestApi } from '@/api/leave-request';
import type { LeaveRequestStatus } from '@/core/types/leave-request';

type UpdateStatusParams = {
  id: string;
  status: LeaveRequestStatus;
};

export function useUpdateStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, status }: UpdateStatusParams) => {
      return leaveRequestApi.updateStatus(id, status);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['leaveRequests'] });
    },
    onError: (error) => {
      console.error('Failed to update leave request status:', error);
    },
  });
}