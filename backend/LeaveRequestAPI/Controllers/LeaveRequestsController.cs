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
        public async Task<ActionResult<IEnumerable<LeaveRequestDTO>>> GetLeaveRequest()
        {
            // obtener el userId del header
            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) || 
                string.IsNullOrEmpty(userIdHeader) ||
                !int.TryParse(userIdHeader, out int userId) ||
                userId <= 0)
            {
                _logger.LogWarning("Missing or invalid X-User-Id header");
                return BadRequest("Se requiere un header X-User-Id válido");
            }

            // obtener el rol del usuario del header
            if (!Request.Headers.TryGetValue("X-User-Role", out var userRoleHeader) || 
                string.IsNullOrEmpty(userRoleHeader))
            {
                _logger.LogWarning("Missing X-User-Role header for GET request");
                return BadRequest("Se requiere el header X-User-Role");
            }

            string userRole = userRoleHeader.ToString();

            _logger.LogInformation("GET leave requests for user {UserId} with role {UserRole}", userId, userRole);

            IEnumerable<LeaveRequestDTO> leaveRequests;

            // validar role y determinar qué datos retornar
            if (userRole == "Manager")
            {
                // Los managers pueden ver todas las solicitudes
                leaveRequests = await _service.GetAllAsync();
            }
            else
            {
                // Los empleados solo pueden ver sus propias solicitudes
                leaveRequests = await _service.GetAllAsync(userId);
            }

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
        public async Task<IActionResult> PutLeaveRequest(int id, LeaveRequestUpdateStatusDTO dto)
        {
            _logger.LogInformation("PUT leave request status {LeaveRequestId}", id);

            // obtener el rol del usuario del header
            if (!Request.Headers.TryGetValue("X-User-Role", out var userRoleHeader) || 
                string.IsNullOrEmpty(userRoleHeader))
            {
                _logger.LogWarning("Missing X-User-Role header for PUT request");
                return BadRequest("Se requiere el header X-User-Role");
            }

            string userRole = userRoleHeader.ToString();

            // validar que solo los managers pueden actualizar el status
            if (userRole != "Manager")
            {
                _logger.LogWarning("Unauthorized PUT attempt for leave request {LeaveRequestId}. User role: {UserRole}", id, userRole);
                return Forbid("Solo los managers pueden actualizar el status de las solicitudes");
            }

            var result = await _service.UpdateStatusAsync(id, dto);

            if (result == null)
            {
                _logger.LogWarning("PUT leave request {LeaveRequestId} not found", id);
                return NotFound();
            }

            return Ok(result);
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

            var result = await _service.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("DELETE leave request {LeaveRequestId} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
