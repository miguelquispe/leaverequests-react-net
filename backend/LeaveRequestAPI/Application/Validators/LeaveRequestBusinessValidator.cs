using LeaveRequestAPI.Application.Common;
using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Domain.Enums;
using LeaveRequestAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveRequestAPI.Application.Validators;

public class LeaveRequestBusinessValidator
{
    private readonly AppDbContext _context;

    public LeaveRequestBusinessValidator(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> ValidateCreateAsync(LeaveRequestCreateDTO dto)
    {
        // 1. Validar que el empleado existe
        var employeeExists = await _context.Employee
            .AnyAsync(e => e.Id == dto.EmployeeId);
        
        if (!employeeExists)
        {
            return Result.Failure(
                $"Employee with ID {dto.EmployeeId} does not exist.",
                BusinessErrorCodes.EMPLOYEE_NOT_FOUND);
        }

        // 2. Validar que la fecha de inicio no sea en el pasado
        if (dto.StartDate.Date < DateTime.Now.Date)
        {
            return Result.Failure(
                "Start date cannot be in the past.",
                BusinessErrorCodes.START_DATE_IN_PAST);
        }

        // 3. Validar que el rango de fechas sea válido
        if (dto.EndDate <= dto.StartDate)
        {
            return Result.Failure(
                "End date must be after start date.",
                BusinessErrorCodes.INVALID_DATE_RANGE);
        }

        // 4. Validar que no haya solapamiento con otras solicitudes del mismo empleado
        var hasOverlapping = await _context.LeaveRequest
            .AnyAsync(lr => lr.EmployeeId == dto.EmployeeId &&
                          lr.Status == LeaveStatus.Approved &&
                          ((dto.StartDate >= lr.StartDate && dto.StartDate <= lr.EndDate) ||
                           (dto.EndDate >= lr.StartDate && dto.EndDate <= lr.EndDate) ||
                           (dto.StartDate <= lr.StartDate && dto.EndDate >= lr.EndDate)));

        if (hasOverlapping)
        {
            return Result.Failure(
                "The requested dates overlap with an existing approved leave request.",
                BusinessErrorCodes.OVERLAPPING_REQUEST);
        }

        return Result.Success();
    }

    public async Task<Result> ValidateUpdateStatusAsync(int leaveRequestId, LeaveRequestUpdateStatusDTO dto)
    {
        // 1. Validar que la solicitud existe
        var leaveRequest = await _context.LeaveRequest
            .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId);

        if (leaveRequest == null)
        {
            return Result.Failure(
                $"Leave request with ID {leaveRequestId} does not exist.",
                BusinessErrorCodes.REQUEST_NOT_FOUND);
        }

        // 2. Validar transiciones de estado válidas
        if (leaveRequest.Status != LeaveStatus.Pending)
        {
            return Result.Failure(
                $"Cannot change status of a request that is already {leaveRequest.Status}.",
                BusinessErrorCodes.INVALID_STATUS_TRANSITION);
        }

        return Result.Success();
    }

    public async Task<Result> ValidateDeleteAsync(int leaveRequestId)
    {
        // 1. Validar que la solicitud existe
        var leaveRequest = await _context.LeaveRequest
            .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId);

        if (leaveRequest == null)
        {
            return Result.Failure(
                $"Leave request with ID {leaveRequestId} does not exist.",
                BusinessErrorCodes.REQUEST_NOT_FOUND);
        }

        // 2. Validar que solo se pueden eliminar solicitudes pendientes
        if (leaveRequest.Status == LeaveStatus.Approved)
        {
            return Result.Failure(
                "Cannot delete an approved leave request.",
                BusinessErrorCodes.CANNOT_DELETE_APPROVED_REQUEST);
        }

        return Result.Success();
    }
}
