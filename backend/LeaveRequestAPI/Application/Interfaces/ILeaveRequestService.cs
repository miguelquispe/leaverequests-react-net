using LeaveRequestAPI.Application.Common;
using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Domain.Entities;

namespace LeaveRequestAPI.Application.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<Result<IEnumerable<LeaveRequestDTO>>> GetAllAsync(int? userId = null);
        Task<Result<LeaveRequestDTO>> CreateAsync(LeaveRequestCreateDTO request);
        Task<Result<LeaveRequestDTO>> UpdateStatusAsync(int id, LeaveRequestUpdateStatusDTO dto);
        Task<Result> DeleteAsync(int id);
    }
}
