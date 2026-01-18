import {
  createContext,
  useContext,
  useState,
  useEffect,
  type ReactNode,
} from "react";
import type { Employee } from "@/core/types/employee";

interface AuthContextType {
  user: Employee | null;
  isAuthenticated: boolean;
  login: (employee: Employee) => void;
  logout: () => void;
  updateUser: (user: Employee) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<Employee | null>(null);

  useEffect(() => {
    // Try to load user from localStorage on mount
    const savedUser = localStorage.getItem("currentUser");
    if (savedUser) {
      try {
        setUser(JSON.parse(savedUser));
      } catch (error) {
        console.error("Failed to parse saved user:", error);
        localStorage.removeItem("currentUser");
      }
    }
  }, []);

  const login = (employee: Employee) => {
    setUser(employee);
    localStorage.setItem("currentUser", JSON.stringify(employee));
    localStorage.setItem("userId", employee.id?.toString() || "");
    localStorage.setItem("userRole", employee.role || "");
    // You might want to set an authToken here too
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem("currentUser");
    localStorage.removeItem("selectedEmployee");
    localStorage.removeItem("currentView"); // Clear navigation state
    localStorage.removeItem("userId");
    localStorage.removeItem("userRole");
    localStorage.removeItem("authToken");
  };

  const updateUser = (updatedUser: Employee) => {
    setUser(updatedUser);
    localStorage.setItem("currentUser", JSON.stringify(updatedUser));
    localStorage.setItem("userId", updatedUser.id?.toString() || "");
    localStorage.setItem("userRole", updatedUser.role || "");
  };

  const value: AuthContextType = {
    user,
    isAuthenticated: user !== null,
    login,
    logout,
    updateUser,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
