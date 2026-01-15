import { useMutation, useQueryClient } from '@tanstack/react-query';
import { leaveRequestApi } from '@/api/leave-request';

export function useDeleteLeaveRequest() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => {
      return leaveRequestApi.delete(id);
    },
    onSuccess: () => {
      // Invalidate and refetch leave requests
      queryClient.invalidateQueries({ queryKey: ['leaveRequests'] });
    },
    onError: (error) => {
      console.error('Failed to delete leave request:', error);
    },
  });
}