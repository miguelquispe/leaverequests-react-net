using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Application.Interfaces;
using LeaveRequestAPI.Domain.Entities;
using LeaveRequestAPI.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveRequestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LeaveRequestsController> _logger;
        private readonly ILeaveRequestService _service;

        public LeaveRequestsController(AppDbContext context, ILogger<LeaveRequestsController> logger, ILeaveRequestService service)
        {
            _context = context;
            _logger = logger;
            _service = service;
        }

        // GET: api/LeaveRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestDTO>>> GetLeaveRequest([FromQuery] int? userId, [FromQuery] string userRole)
        {

            // validar role y userId (empleado)
            if (!userId.HasValue || userId <= 0 || string.IsNullOrEmpty(userRole) )
            {
                _logger.LogWarning("Missing or invalid X-UserId and X-User-Role header");
                return BadRequest("Faltan datos del usuario: Id y Role");
            }

            int employeeId = userId.Value;
            string role = userRole.ToString();

            _logger.LogInformation("GET all leave requests");
            var leaveRequests = await _service.GetAllAsync(employeeId, role!);
            return Ok(leaveRequests);
        }

        // GET: api/LeaveRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequest>> GetLeaveRequest(int id)
        {
            _logger.LogInformation("GET leave request {LeaveRequestId}", id);

            var leaveRequest = await _context.LeaveRequest.FindAsync(id);

            if (leaveRequest == null)
            {
                _logger.LogWarning("Leave request {LeaveRequestId} not found", id);
                return NotFound();
            }

            return leaveRequest;
        }

        // PUT: api/LeaveRequests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaveRequest(int id, LeaveRequest leaveRequest)
        {
            _logger.LogInformation("PUT leave request {LeaveRequestId}", id);

            if (id != leaveRequest.Id)
            {
                _logger.LogWarning("PUT leave request id mismatch: {LeaveRequestId}", id);
                return BadRequest();
            }

            _context.Entry(leaveRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveRequestExists(id))
                {
                    _logger.LogWarning("PUT leave request {LeaveRequestId} not found", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("PUT concurrency error for leave request {LeaveRequestId}", id);
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LeaveRequests
        [HttpPost]
        public async Task<ActionResult<LeaveRequestDTO>> PostLeaveRequest([FromBody] LeaveRequestCreateDTO dto)
        {
            _logger.LogInformation("POST new leave request");
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        // DELETE: api/LeaveRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            _logger.LogInformation("DELETE leave request {LeaveRequestId}", id);

            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
            if (leaveRequest == null)
            {
                _logger.LogWarning("DELETE leave request {LeaveRequestId} not found", id);
                return NotFound();
            }

            _context.LeaveRequest.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequest.Any(e => e.Id == id);
        }
    }
}
