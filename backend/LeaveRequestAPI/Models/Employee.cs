using LeaveRequestAPI.Enums;

namespace LeaveRequestAPI.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
}
