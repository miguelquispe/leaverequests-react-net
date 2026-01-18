import { useDeleteLeaveRequest, useUpdateStatus } from "./";

interface UseLeaveRequestActionsProps {
  onSuccess: (message: string) => void;
  onError: (message: string) => void;
}

export function useLeaveRequestActions({
  onSuccess,
  onError,
}: UseLeaveRequestActionsProps) {
  const deleteRequest = useDeleteLeaveRequest();
  const updateStatus = useUpdateStatus();

  const handleDelete = async (id?: string) => {
    if (!id) return;
    if (confirm("Are you sure you want to delete this leave request?")) {
      try {
        await deleteRequest.mutateAsync(id);
        onSuccess("Leave request deleted successfully");
      } catch (error) {
        console.log("Error deleting leave request:", error);
        onError("Error deleting leave request");
      }
    }
  };

  const handleApprove = async (id?: string) => {
    if (!id) return;
    try {
      await updateStatus.mutateAsync({ id, status: { status: "Approved" } });
      onSuccess("Leave request approved");
    } catch (error) {
        console.log("Error approving leave request:", error);
      onError("Error approving leave request");
    }
  };

  const handleReject = async (id?: string) => {
    if (!id) return;
    if (confirm("Are you sure you want to reject this leave request?")) {
      try {
        await updateStatus.mutateAsync({ id, status: { status: "Rejected" } });
        onSuccess("Leave request rejected");
      } catch (error) {
        console.log("Error rejecting leave request:", error);
        onError("Error rejecting leave request");
      }
    }
  };

  return {
    handleDelete,
    handleApprove,
    handleReject,
    isDeleting: deleteRequest.isPending,
    isUpdating: updateStatus.isPending,
  };
}