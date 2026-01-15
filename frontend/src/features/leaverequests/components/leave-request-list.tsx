import { useState, useEffect } from "react";
import type { LeaveRequest } from "@/core/types/leave-request";
// If you need modified types for ui
// import type { LeaveRequestFeature } from "../types";

// Mock data for demonstration - replace with actual API call
const mockLeaveRequests: LeaveRequest[] = [
  {
    id: 1,
    employeeName: "John Doe",
    startDate: "2026-01-20T00:00:00",
    endDate: "2026-01-22T00:00:00",
    status: "Pending",
    reason: "Family vacation",
  },
  {
    id: 2,
    employeeName: "Jane Smith",
    startDate: "2026-02-01T00:00:00",
    endDate: "2026-02-03T00:00:00",
    status: "Approved",
    reason: "Medical appointment",
  },
  {
    id: 3,
    employeeName: "Mike Johnson",
    startDate: "2026-01-25T00:00:00",
    endDate: "2026-01-26T00:00:00",
    status: "Rejected",
    reason: "Personal day",
  },
];

export function LeaveRequestList() {
  const [leaveRequests, setLeaveRequests] = useState<LeaveRequest[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // TODO: Replace with actual API call
    setTimeout(() => {
      setLeaveRequests(mockLeaveRequests);
      setLoading(false);
    }, 1000);
  }, []);

  const formatDate = (dateString?: string) => {
    if (!dateString) return "-";
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  };

  const getStatusColor = (status?: string) => {
    switch (status?.toLowerCase()) {
      case "approved":
        return "bg-green-100 text-green-800 border-green-200";
      case "rejected":
        return "bg-red-100 text-red-800 border-red-200";
      case "pending":
      default:
        return "bg-yellow-100 text-yellow-800 border-yellow-200";
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-8">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span className="ml-2 text-gray-600">Loading leave requests...</span>
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
    <div className="bg-white rounded-lg shadow-md overflow-hidden">
      <div className="px-6 py-4 border-b border-gray-200">
        <h2 className="text-xl font-semibold text-gray-800">Leave Requests</h2>
      </div>

      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                ID
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Employee
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Start Date
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                End Date
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Status
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Reason
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {leaveRequests.map((request) => (
              <tr
                key={request.id}
                className="hover:bg-gray-50 transition-colors"
              >
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                  #{request.id}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {request.employeeName || "-"}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {formatDate(request.startDate)}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {formatDate(request.endDate)}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <span
                    className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full border ${getStatusColor(
                      request.status || "pending"
                    )}`}
                  >
                    {request.status || "Unknown"}
                  </span>
                </td>
                <td className="px-6 py-4 text-sm text-gray-900 max-w-xs truncate">
                  {request.reason || "-"}
                </td>
                <td>
                  <button>APPROVE</button>
                  <button>REJECTED</button>
                  <button>DELETE</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
