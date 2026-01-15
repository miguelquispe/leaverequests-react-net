import { LeaveRequestForm } from "./features/leaverequests/components/leave-request-form";
import { LeaveRequestList } from "./features/leaverequests/components/leave-request-list";

function App() {
  return (
    <>
      <h1 className="text-3xl font-bold underline">Leave Request</h1>
      <LeaveRequestForm />
      <LeaveRequestList />
    </>
  );
}

export default App;
