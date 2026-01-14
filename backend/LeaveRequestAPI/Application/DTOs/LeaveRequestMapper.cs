using AutoMapper;
using LeaveRequestAPI.Domain.Entities;

namespace LeaveRequestAPI.Application.DTOs;

public class LeaveRequestMapper: Profile
{
    public LeaveRequestMapper() { 
        // Lectura
        CreateMap<LeaveRequest, LeaveRequestDTO>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : string.Empty));

        // Escritura
        CreateMap<LeaveRequestCreateDTO, LeaveRequest>().ForMember(dest => dest.Status, opt => opt.Ignore());
    }
}
