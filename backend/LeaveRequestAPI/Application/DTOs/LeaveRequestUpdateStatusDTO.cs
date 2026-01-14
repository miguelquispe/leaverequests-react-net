using LeaveRequestAPI.Domain.Enums;

namespace LeaveRequestAPI.Application.DTOs;

public class LeaveRequestUpdateStatusDTO
{
    public LeaveStatus Status { get; set; }
}
