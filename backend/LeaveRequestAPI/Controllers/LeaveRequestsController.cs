using LeaveRequestAPI.Application.Common;
using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Application.Extensions;
using LeaveRequestAPI.Application.Filters;
using LeaveRequestAPI.Application.Interfaces;
using LeaveRequestAPI.Domain.Entities;
using LeaveRequestAPI.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace LeaveRequestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LeaveRequestsController> _logger;
        private readonly ILeaveRequestService _service;
        private readonly IValidator<LeaveRequestCreateDTO> _createValidator;
        private readonly IValidator<LeaveRequestUpdateStatusDTO> _updateValidator;

        public LeaveRequestsController(
            AppDbContext context, 
            ILogger<LeaveRequestsController> logger, 
            ILeaveRequestService service,
            IValidator<LeaveRequestCreateDTO> createValidator,
            IValidator<LeaveRequestUpdateStatusDTO> updateValidator)
        {
            _context = context;
            _logger = logger;
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // GET: api/LeaveRequests
        [HttpGet]
        [ValidateUserAuthentication]
        public async Task<ActionResult<IEnumerable<LeaveRequestDTO>>> GetLeaveRequest()
        {
            var authenticatedUser = HttpContext.GetAuthenticatedUser()!;
            
            _logger.LogInformation("GET leave requests for user {UserId} with role {UserRole}", 
                authenticatedUser.UserId, authenticatedUser.Role);

            Result<IEnumerable<LeaveRequestDTO>> leaveRequestsResult;

            // validar role y determinar qué datos retornar
            if (authenticatedUser.Role == Domain.Enums.EmployeeRole.Manager)
            {
                // Los managers pueden ver todas las solicitudes
                leaveRequestsResult = await _service.GetAllAsync();
            }
            else
            {
                // Los empleados solo pueden ver sus propias solicitudes
                leaveRequestsResult = await _service.GetAllAsync(authenticatedUser.UserId);
            }

            if (!leaveRequestsResult.IsSuccess)
            {
                _logger.LogError("Error getting leave requests: {ErrorMessage}", leaveRequestsResult.ErrorMessage);
                return StatusCode(500, new { 
                    message = leaveRequestsResult.ErrorMessage,
                    code = leaveRequestsResult.ErrorCode 
                });
            }

            return Ok(leaveRequestsResult.Data);
        }

        // GET: api/LeaveRequests/5
        // [HttpGet("{id}")]
        // public async Task<ActionResult<LeaveRequest>> GetLeaveRequest(int id)
        // {
        //     _logger.LogInformation("GET leave request {LeaveRequestId}", id);

        //     var leaveRequest = await _context.LeaveRequest.FindAsync(id);

        //     if (leaveRequest == null)
        //     {
        //         _logger.LogWarning("Leave request {LeaveRequestId} not found", id);
        //         return NotFound();
        //     }

        //     return leaveRequest;
        // }

        // PUT: api/LeaveRequests/5
        [HttpPut("{id}")]
        [ValidateUserAuthentication]
        public async Task<IActionResult> PutLeaveRequest(int id, LeaveRequestUpdateStatusDTO dto)
        {
            _logger.LogInformation("PUT leave request status {LeaveRequestId}", id);

            var authenticatedUser = HttpContext.GetAuthenticatedUser()!;

            // Validar el DTO
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = errors
                });
            }

            // validar que solo los managers pueden actualizar el status
            if (authenticatedUser.Role != Domain.Enums.EmployeeRole.Manager)
            {
                _logger.LogWarning("Unauthorized PUT attempt for leave request {LeaveRequestId}. User role: {UserRole}", 
                    id, authenticatedUser.Role);
                return Forbid("Solo los managers pueden actualizar el status de las solicitudes");
            }

            var result = await _service.UpdateStatusAsync(id, dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update leave request {LeaveRequestId}: {ErrorMessage}", id, result.ErrorMessage);
                
                // Mapear códigos de error a códigos HTTP apropiados
                return result.ErrorCode switch
                {
                    BusinessErrorCodes.REQUEST_NOT_FOUND => NotFound(new { 
                        message = result.ErrorMessage, 
                        code = result.ErrorCode 
                    }),
                    BusinessErrorCodes.INVALID_STATUS_TRANSITION => BadRequest(new { 
                        message = result.ErrorMessage, 
                        code = result.ErrorCode 
                    }),
                    _ => StatusCode(500, new { 
                        message = result.ErrorMessage, 
                        code = result.ErrorCode 
                    })
                };
            }

            return Ok(result.Data);
        }

        // POST: api/LeaveRequests
        [HttpPost]
        [ValidateUserAuthentication]
        public async Task<ActionResult<LeaveRequestDTO>> PostLeaveRequest([FromBody] LeaveRequestCreateDTO dto)
        {
            _logger.LogInformation("POST new leave request");

            // Validar el DTO
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = errors
                });
            }

            var result = await _service.CreateAsync(dto);
            
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create leave request: {ErrorMessage}", result.ErrorMessage);
                
                // Mapear códigos de error a códigos HTTP apropiados
                return result.ErrorCode switch
                {
                    BusinessErrorCodes.EMPLOYEE_NOT_FOUND => BadRequest(new { 
                        message = result.ErrorMessage, 
                        code = result.ErrorCode 
                    }),
                    BusinessErrorCodes.START_DATE_IN_PAST or 
                    BusinessErrorCodes.INVALID_DATE_RANGE or 
                    BusinessErrorCodes.OVERLAPPING_REQUEST => BadRequest(new { 
                        message = result.ErrorMessage, 
                        code = result.ErrorCode 
                    }),
                    _ => StatusCode(500, new { 
                        message = result.ErrorMessage, 
                        code = result.ErrorCode 
                    })
                };
            }

            return Created("", result.Data);
        }

        // DELETE: api/LeaveRequests/5
        [HttpDelete("{id}")]
        [ValidateUserAuthentication]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            _logger.LogInformation("DELETE leave request {LeaveRequestId}", id);

            var result = await _service.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete leave request {LeaveRequestId}: {ErrorMessage}", id, result.ErrorMessage);
                
                // Mapear códigos de error a códigos HTTP apropiados
                return result.ErrorCode switch
                {
                    BusinessErrorCodes.REQUEST_NOT_FOUND => NotFound(new { 
                        message = result.ErrorMessage, 
                        code = result.ErrorCode 
                    }),
                    BusinessErrorCodes.CANNOT_DELETE_APPROVED_REQUEST => BadRequest(new { 
                        message = result.ErrorMessage, 
                        code = result.ErrorCode 
                    }),
                    _ => StatusCode(500, new { 
                        message = result.ErrorMessage, 
                        code = result.ErrorCode 
                    })
                };
            }

            return NoContent();
        }
    }
}
