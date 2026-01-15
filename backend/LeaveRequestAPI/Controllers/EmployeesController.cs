using LeaveRequestAPI.Application.Common;
using LeaveRequestAPI.Application.Extensions;
using LeaveRequestAPI.Domain.Entities;
using LeaveRequestAPI.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveRequestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(AppDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Employee>>>> GetEmployees()
        {
            try
            {
                _logger.LogInformation("GET all employees");

                var employees = await _context.Employee
                    .OrderBy(e => e.Name)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} employees", employees.Count);

                return this.ApiSuccess<IEnumerable<Employee>>(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees");
                return this.ApiError<IEnumerable<Employee>>("Internal server error", "INTERNAL_ERROR", 500);
            }
        }

        // GET: api/employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Employee>>> GetEmployee(int id)
        {
            try
            {
                _logger.LogInformation("GET employee with ID {Id}", id);

                var employee = await _context.Employee.FindAsync(id);

                if (employee == null)
                {
                    _logger.LogWarning("Employee with ID {Id} not found", id);
                    return this.ApiError<Employee>("Employee not found", BusinessErrorCodes.EMPLOYEE_NOT_FOUND, 404);
                }

                return this.ApiSuccess(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee with ID {Id}", id);
                return this.ApiError<Employee>("Internal server error", "INTERNAL_ERROR", 500);
            }
        }
    }
}