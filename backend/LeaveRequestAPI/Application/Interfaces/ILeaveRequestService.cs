using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Domain.Entities;

namespace LeaveRequestAPI.Application.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequestDTO>> GetAllAsync(int userId, string role);
        Task<LeaveRequestDTO> CreateAsync(LeaveRequestCreateDTO request);
        Task<LeaveRequestDTO?> UpdateStatusAsync(int id, LeaveRequestUpdateStatusDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
