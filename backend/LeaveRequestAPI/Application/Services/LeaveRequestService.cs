using AutoMapper;
using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Application.Interfaces;
using LeaveRequestAPI.Domain.Entities;
using LeaveRequestAPI.Domain.Enums;
using LeaveRequestAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveRequestAPI.Application.Services
{
    public class LeaveRequestService: ILeaveRequestService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public LeaveRequestService(AppDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LeaveRequestDTO>> GetAllAsync(int userId, string role)
        {
            // consultar la base de datos para obtener datos de empleado
            var query = _context.LeaveRequest.Include(lr => lr.Employee).AsQueryable();

            // validamos role y filtramos por userId si es empleado
            if (role != "Manager")
            {
                query = query.Where(x => x.EmployeeId == userId);
            }

            // ejecutar la consulta
            var entities = await query.ToListAsync();

            // mapear las entidades a DTOs y retornarlas
            return _mapper.Map<IEnumerable<LeaveRequestDTO>>(entities);
        }
        public async Task<LeaveRequestDTO> CreateAsync(LeaveRequestCreateDTO dto)
        {
            // transformar el DTO en una entidad
            var request = _mapper.Map<LeaveRequest>(dto);
            // asignar el estado inicial
            request.Status = LeaveStatus.Pending;

            // guardar en la base de datos
            _context.LeaveRequest.Add(request);
            await  _context.SaveChangesAsync();

            // retornar el DTO resultante
            return _mapper.Map<LeaveRequestDTO>(request);
        }

    }
}
