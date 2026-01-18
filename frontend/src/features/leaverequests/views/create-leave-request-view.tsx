import { useLeaveRequestForm, useMessageHandler } from "../hooks";
import { LeaveRequestFormFields } from "../components/leave-request-form-fields";
import { useApp } from "@/contexts/AppContext";
import { useState } from "react";

export function CreateLeaveRequestView() {
  const { navigateTo } = useApp();
  const { message, messageType, showMessage } = useMessageHandler();
  const [serverErrors, setServerErrors] = useState<string[]>([]);
  const { formData, errors, updateField, submitForm, isLoading } =
    useLeaveRequestForm({
      onSuccess: (msg) => {
        showMessage(msg, "success");
        setTimeout(() => navigateTo("dashboard"), 1500);
      },
      onError: (error) => {
        showMessage(error.message, "error");
        setServerErrors(error.errors);
      },
    });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    submitForm();
  };

  return (
    <div className="max-w-md mx-auto bg-white rounded-lg shadow-md p-6">
      {message && (
        <div
          className={`px-4 py-3 rounded mb-6 ${
            messageType === "success"
              ? "bg-green-50 border border-green-200 text-green-800"
              : "bg-red-50 border border-red-200 text-red-800"
          }`}
        >
          {message}
        </div>
      )}
      <h2 className="text-2xl font-bold text-gray-800 mb-6">
        Create Leave Request
      </h2>
      <LeaveRequestFormFields
        formData={formData}
        errors={errors}
        onFieldChange={updateField}
        onSubmit={handleSubmit}
        isLoading={isLoading}
      />
      {serverErrors && serverErrors.length > 0 && (
        <div className="px-4 py-3 rounded mb-6 bg-red-50 border border-red-200 text-red-800 mt-4">
          <h3 className="font-semibold mb-2">
            Please fix the following errors:
          </h3>
          <ul className="list-disc list-inside">
            {serverErrors.map((message, index) => (
              <li key={index}>{message}</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
