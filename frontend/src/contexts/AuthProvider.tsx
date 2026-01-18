import type { Employee } from "@/core/types/employee";
import { useState, type ReactNode } from "react";
import { AuthContext, type AuthContextType } from "./AuthContext";

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<Employee | null>(() => {
    // Initialize user from localStorage on mount
    const savedUser = localStorage.getItem("currentUser");
    if (savedUser) {
      try {
        return JSON.parse(savedUser);
      } catch (error) {
        console.error("Failed to parse saved user:", error);
        localStorage.removeItem("currentUser");
        return null;
      }
    }
    return null;
  });

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
