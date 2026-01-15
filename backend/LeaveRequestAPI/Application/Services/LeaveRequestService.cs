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

        public async Task<IEnumerable<LeaveRequestDTO>> GetAllAsync(int? userId = null)
        {
            // consultar la base de datos para obtener datos de empleado
            var query = _context.LeaveRequest.Include(lr => lr.Employee).AsQueryable();

            // filtrar por userId si se proporciona (para empleados)
            if (userId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == userId.Value);
            }
            // Si userId es null, retorna todas las solicitudes (para managers)

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

            // cargar el empleado para incluir el nombre en la respuesta
            await _context.Entry(request)
                .Reference(lr => lr.Employee)
                .LoadAsync();

            // retornar el DTO resultante
            return _mapper.Map<LeaveRequestDTO>(request);
        }

        public async Task<LeaveRequestDTO?> UpdateStatusAsync(int id, LeaveRequestUpdateStatusDTO dto)
        {
            // buscar el leave request existente
            var existingRequest = await _context.LeaveRequest
                .Include(lr => lr.Employee)
                .FirstOrDefaultAsync(lr => lr.Id == id);

            if (existingRequest == null)
            {
                return null;
            }

            // actualizar solo el status
            existingRequest.Status = dto.Status;

            // guardar los cambios
            _context.LeaveRequest.Update(existingRequest);
            await _context.SaveChangesAsync();

            // retornar el DTO actualizado
            return _mapper.Map<LeaveRequestDTO>(existingRequest);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // buscar el leave request a eliminar
            var leaveRequest = await _context.LeaveRequest.FindAsync(id);

            if (leaveRequest == null)
            {
                return false;
            }

            // eliminar el registro
            _context.LeaveRequest.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
