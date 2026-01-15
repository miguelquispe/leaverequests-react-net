
import type { Employee, EmployeeRole } from '@/core/types/employee';

// Utility to convert API Employee to EmployeesEmployee
export const mapEmployeeToEmployees = (employee: Employee): Employee => ({
  id: employee.id || 0,
  name: employee.name || 'No name',
  email: employee.email || 'No email',
  role: employee.role || 'Employee',
});

export type { EmployeeRole };