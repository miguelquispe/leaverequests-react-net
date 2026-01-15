import { useQuery } from '@tanstack/react-query';
import { leaveRequestApi } from '@/api/leave-request';

export function useGetLeaveRequestById(id: string) {
  return useQuery({
    queryKey: ['leaveRequests', id],
    queryFn: () => leaveRequestApi.getById(id),
    enabled: !!id, // Only run query if id is provided
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
}