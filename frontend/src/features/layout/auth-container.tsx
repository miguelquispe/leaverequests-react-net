import { useAuth } from "@/contexts/AuthContext";
import React from "react";

function AuthContainer({ children }: { children: React.ReactNode }) {
  const { user, logout, isAuthenticated } = useAuth();
  return (
    <div>
      {isAuthenticated && (
        <div className="flex gap-4 items-center justify-end mb-4 px-6 py-4 border-b border-gray-100  shadow-neutral-200 bg-gray-200">
          <div className="text-sm text-gray-600">
            <span className="font-medium">{user?.name}</span>
            <span className="mx-2">•</span>
            <span className="capitalize">{user?.role}</span>
          </div>
          <button
            onClick={logout}
            className="px-4 py-2 bg-gray-500 text-white rounded"
          >
            Logout
          </button>
        </div>
      )}
      <div className="max-w-5xl mx-auto">{children}</div>
    </div>
  );
}

export default AuthContainer;
