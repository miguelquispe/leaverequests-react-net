import {
  createContext,
  useContext,
  useState,
  type ReactNode,
  useCallback,
  useMemo,
  useEffect,
} from "react";

type AppView =
  | "employee-selection"
  | "dashboard"
  | "create-leave-request"
  | "leave-history"
  | "manage-requests";

interface AppContextType {
  currentView: AppView;
  setCurrentView: (view: AppView) => void;
  navigateTo: (view: AppView) => void;
  goBack: () => void;
}

const AppContext = createContext<AppContextType | undefined>(undefined);

interface AppProviderProps {
  children: ReactNode;
}

export function AppProvider({ children }: AppProviderProps) {
  const [currentView, setCurrentView] = useState<AppView>(() => {
    // Initialize from localStorage if available, fallback to employee-selection
    const saved = localStorage.getItem("currentView");
    return (saved as AppView) || "employee-selection";
  });
  const [viewHistory, setViewHistory] = useState<AppView[]>([]);

  // Persist current view to localStorage
  useEffect(() => {
    localStorage.setItem("currentView", currentView);
  }, [currentView]);

  const navigateTo = useCallback(
    (view: AppView) => {
      setViewHistory((prev) => [...prev, currentView]);
      setCurrentView(view);
    },
    [currentView]
  );

  const goBack = useCallback(() => {
    const previousView = viewHistory[viewHistory.length - 1];
    if (previousView) {
      setViewHistory((prev) => prev.slice(0, -1));
      setCurrentView(previousView);
    }
  }, [viewHistory]);

  const setCurrentViewOptimized = useCallback(
    (view: AppView) => {
      if (view !== currentView) {
        setCurrentView(view);
      }
    },
    [currentView]
  );

  const value: AppContextType = useMemo(
    () => ({
      currentView,
      setCurrentView: setCurrentViewOptimized,
      navigateTo,
      goBack,
    }),
    [currentView, setCurrentViewOptimized, navigateTo, goBack]
  );

  return <AppContext.Provider value={value}>{children}</AppContext.Provider>;
}

export function useApp() {
  const context = useContext(AppContext);
  if (context === undefined) {
    throw new Error("useApp must be used within an AppProvider");
  }
  return context;
}
