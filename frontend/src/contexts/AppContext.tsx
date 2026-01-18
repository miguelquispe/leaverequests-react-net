import { createContext, useContext } from "react";

export type AppView =
  | "employee-selection"
  | "dashboard"
  | "create-leave-request"
  | "leave-history"
  | "manage-requests";

export interface AppContextType {
  currentView: AppView;
  setCurrentView: (view: AppView) => void;
  navigateTo: (view: AppView) => void;
  goBack: () => void;
}

export const AppContext = createContext<AppContextType | undefined>(undefined);

export function useApp() {
  const context = useContext(AppContext);
  if (context === undefined) {
    throw new Error("useApp must be used within an AppProvider");
  }
  return context;
}
