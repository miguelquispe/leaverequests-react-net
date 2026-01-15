using LeaveRequestAPI.Domain.Enums;

namespace LeaveRequestAPI.Application.Common;

public class AuthenticatedUser
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
}
