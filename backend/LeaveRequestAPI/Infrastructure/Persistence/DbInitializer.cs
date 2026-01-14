using LeaveRequestAPI.Domain.Entities;
using LeaveRequestAPI.Domain.Enums;

namespace LeaveRequestAPI.Infrastructure.Persistence;

public class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Employee.Any() || context.LeaveRequest.Any())
        {
            return; // DB has been seeded
        }

        // Seed Employees
        var employees = new[]
        {
            new Employee { Name = "Ana García", Email = "ana.garcia@email.com", Role = EmployeeRole.Employee },
            new Employee { Name = "Noah Salvador", Email = "noah.salvador@email.com", Role = EmployeeRole.Manager },
            new Employee { Name = "Marta López", Email = "marta.lopez@email.com", Role = EmployeeRole.Employee },
            new Employee { Name = "Shelia Villalobos", Email = "shelia.villalobos@email.com", Role = EmployeeRole.Manager },

        };

        context.Employee.AddRange(employees);
        context.SaveChanges();

        // Seed LeaveRequests
        var leaveRequests = new[]
        {
            new LeaveRequest
            {
                EmployeeId = employees[0].Id,
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(5),
                Status = LeaveStatus.Pending,
                Reason = "Vacaciones"
            },
            new LeaveRequest
            {
                EmployeeId = employees[1].Id,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(12),
                Status = LeaveStatus.Approved,
                Reason = "Asuntos personales"
            },
            new LeaveRequest
            {
                EmployeeId = employees[2].Id,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3),
                Status = LeaveStatus.Rejected,
                Reason = "Enfermedad"
            }
        };

        context.LeaveRequest.AddRange(leaveRequests);
        context.SaveChanges();
    }
}
