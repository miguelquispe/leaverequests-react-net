using LeaveRequestAPI.Domain.Enums;

namespace LeaveRequestAPI.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
}
