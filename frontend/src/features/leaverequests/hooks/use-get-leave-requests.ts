import { useQuery } from '@tanstack/react-query';
import { useAuth } from '@/contexts/AuthContext';
import { leaveRequestApi } from '@/api/leave-request';

export function useGetLeaveRequests() {
  const { user, isAuthenticated } = useAuth();

  return useQuery({
    queryKey: ['leaveRequests', user?.id],
    queryFn: () => leaveRequestApi.getAll(),
    staleTime: 5 * 60 * 1000, // 5 minutes
    refetchOnWindowFocus: false,
    enabled: isAuthenticated && !!user?.id, // Only fetch when user is authenticated
  });
}