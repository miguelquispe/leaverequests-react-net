import {
  useGetLeaveRequests,
  useMessageHandler,
  useLeaveRequestActions,
} from "../hooks";
import { LeaveRequestTable } from "../components/leave-request-table";

export function LeaveRequestsView() {
  const { data: response, isLoading, error, refetch } = useGetLeaveRequests();
  const leaveRequests = response?.data || [];
  const { message, messageType, showMessage } = useMessageHandler();
  const { handleDelete, handleApprove, handleReject, isDeleting, isUpdating } =
    useLeaveRequestActions({
      onSuccess: (msg) => showMessage(msg, "success"),
      onError: (msg) => showMessage(msg, "error"),
    });

  if (isLoading) {
    return (
      <div className="flex justify-center items-center py-8">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span className="ml-2 text-gray-600">Loading leave requests...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-8">
        <p className="text-red-500 text-lg">Error: {error.message}</p>
        <button
          onClick={() => refetch()}
          className="mt-4 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          Try Again
        </button>
      </div>
    );
  }

  if (leaveRequests.length === 0) {
    return (
      <div className="text-center py-8">
        <p className="text-gray-500 text-lg">No leave requests found</p>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-lg shadow-md overflow-hidden w-full">
      {message && (
        <div
          className={`px-6 py-3 border-b ${
            messageType === "success"
              ? "bg-green-50 border-green-200 text-green-800"
              : "bg-red-50 border-red-200 text-red-800"
          }`}
        >
          {message}
        </div>
      )}
      <div className="px-6 py-4 border-b border-gray-200">
        <h2 className="text-xl font-semibold text-gray-800">Leave Requests</h2>
      </div>

      <LeaveRequestTable
        leaveRequests={leaveRequests}
        onDelete={handleDelete}
        onApprove={handleApprove}
        onReject={handleReject}
        isLoading={isDeleting || isUpdating}
      />
    </div>
  );
}
