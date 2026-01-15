import { useQuery } from '@tanstack/react-query';
import { leaveRequestApi } from '@/api/leave-request';

export function useGetLeaveRequests() {
  return useQuery({
    queryKey: ['leaveRequests'],
    queryFn: () => leaveRequestApi.getAll(),
    staleTime: 5 * 60 * 1000, // 5 minutes
    refetchOnWindowFocus: false,
  });
}