import { useMemo, useCallback, useEffect, type JSX } from "react";
import { useAuth } from "./contexts/AuthContext";
import { useApp } from "./contexts/AppContext";
import { EmployeesView } from "./features/employees";
import DashboardPage from "./pages/dashboard-page";
import AuthContainer from "./features/layout/auth-container";
import { CreateLeaveRequestView } from "./features/leaverequests/views/create-leave-request-view";
import type { Employee } from "./core/types/employee";

function App() {
  const { isAuthenticated, login } = useAuth();
  const { currentView, setCurrentView, navigateTo } = useApp();

  // Sync authentication state with navigation
  useEffect(() => {
    if (isAuthenticated && currentView === "employee-selection") {
      setCurrentView("dashboard");
    } else if (!isAuthenticated && currentView !== "employee-selection") {
      setCurrentView("employee-selection");
    }
  }, [isAuthenticated, currentView, setCurrentView]);

  // Optimize handlers with useCallback
  const handleEmployeeSelect = useCallback(
    (employee: Employee) => {
      login(employee);
      navigateTo("dashboard");
    },
    [login, navigateTo]
  );

  const handleBackToDashboard = useCallback(() => {
    navigateTo("dashboard");
  }, [navigateTo]);

  // Memoize views to prevent unnecessary re-renders
  const views: Record<string, JSX.Element> = useMemo(
    () => ({
      "employee-selection": (
        <EmployeesView onEmployeeSelect={handleEmployeeSelect} />
      ),
      dashboard: (
        <AuthContainer>
          <DashboardPage />
        </AuthContainer>
      ),
      "create-leave-request": (
        <AuthContainer>
          <div className="p-6 max-w-2xl mx-auto">
            <div className="mb-4">
              <button
                onClick={handleBackToDashboard}
                className="px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600 transition-colors mb-4"
              >
                ← Back to Dashboard
              </button>
            </div>
            <CreateLeaveRequestView />
          </div>
        </AuthContainer>
      ),
    }),
    [handleEmployeeSelect, handleBackToDashboard]
  );

  return <div className="min-h-screen bg-gray-100">{views[currentView]}</div>;
}

export default App;
