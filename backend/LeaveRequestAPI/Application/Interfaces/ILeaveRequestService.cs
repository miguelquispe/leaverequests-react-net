using LeaveRequestAPI.Application.Common;
using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Domain.Entities;
using LeaveRequestAPI.Domain.Enums;

namespace LeaveRequestAPI.Application.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<Result<IEnumerable<LeaveRequestDTO>>> GetAllAsync(int? userId = null);
        Task<Result<LeaveRequestDTO>> CreateAsync(LeaveRequestCreateDTO request);
        Task<Result<LeaveRequestDTO>> UpdateStatusAsync(int id, LeaveRequestUpdateStatusDTO dto, EmployeeRole userRole);
        Task<Result> DeleteAsync(int id);
    }
}
