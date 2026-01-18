import { useApp } from "@/contexts/AppContext";
import { LeaveRequestsView } from "@/features/leaverequests/views/leave-requests-view";

function DashboardPage() {
  const { navigateTo } = useApp();

  const handleCreateClick = () => {
    navigateTo("create-leave-request");
  };

  return (
    <div className="p-6 max-w-5xl mx-auto">
      <div className="mb-4 flex justify-between">
        <h1 className="text-2xl font-bold">Dashboard</h1>
        <button
          onClick={handleCreateClick}
          className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition-colors"
        >
          Create Leave Request
        </button>
      </div>
      <LeaveRequestsView />
    </div>
  );
}

export default DashboardPage;
