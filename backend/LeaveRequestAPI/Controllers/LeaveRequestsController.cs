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
        public async Task<ActionResult<ApiResponse<IEnumerable<LeaveRequestDTO>>>> GetLeaveRequest()
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
            }

            return this.ApiFromResult<IEnumerable<LeaveRequestDTO>>(leaveRequestsResult);
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
        public async Task<ActionResult<ApiResponse<LeaveRequestDTO>>> PutLeaveRequest(int id, LeaveRequestUpdateStatusDTO dto)
        {
            _logger.LogInformation("PUT leave request status {LeaveRequestId}", id);

            var authenticatedUser = HttpContext.GetAuthenticatedUser()!;

            // Validar el DTO
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return this.ApiValidationError<LeaveRequestDTO>(validationResult);
            }

            // validar que solo los managers pueden actualizar el status - MOVIDO AL BUSINESS VALIDATOR
            // La validación de rol ahora se maneja en el BusinessValidator
            
            var result = await _service.UpdateStatusAsync(id, dto, authenticatedUser.Role);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update leave request {LeaveRequestId}: {ErrorMessage}", id, result.ErrorMessage);
            }

            return this.ApiFromResult<LeaveRequestDTO>(result);
        }

        // POST: api/LeaveRequests
        [HttpPost]
        [ValidateUserAuthentication]
        public async Task<ActionResult<ApiResponse<LeaveRequestDTO>>> PostLeaveRequest([FromBody] LeaveRequestCreateDTO dto)
        {
            _logger.LogInformation("POST new leave request");

            // Validar el DTO
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return this.ApiValidationError<LeaveRequestDTO>(validationResult);
            }

            var result = await _service.CreateAsync(dto);
            
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create leave request: {ErrorMessage}", result.ErrorMessage);
            }

            return this.ApiFromResult<LeaveRequestDTO>(result, 201);
        }

        // DELETE: api/LeaveRequests/5
        [HttpDelete("{id}")]
        [ValidateUserAuthentication]
        public async Task<ActionResult<ApiResponse>> DeleteLeaveRequest(int id)
        {
            _logger.LogInformation("DELETE leave request {LeaveRequestId}", id);

            var result = await _service.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete leave request {LeaveRequestId}: {ErrorMessage}", id, result.ErrorMessage);
                
                return this.ApiFromResult(result);
            }

            // DELETE exitoso retorna 204 No Content sin cuerpo
            return NoContent();
        }
    }
}
