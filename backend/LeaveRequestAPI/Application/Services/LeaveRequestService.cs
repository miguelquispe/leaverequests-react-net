using AutoMapper;
using LeaveRequestAPI.Application.Common;
using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Application.Interfaces;
using LeaveRequestAPI.Application.Validators;
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
        private readonly LeaveRequestBusinessValidator _businessValidator;

        public LeaveRequestService(AppDbContext context, IMapper mapper, LeaveRequestBusinessValidator businessValidator) 
        {
            _context = context;
            _mapper = mapper;
            _businessValidator = businessValidator;
        }

        public async Task<Result<IEnumerable<LeaveRequestDTO>>> GetAllAsync(int? userId = null)
        {
            try
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
                var dtos = _mapper.Map<IEnumerable<LeaveRequestDTO>>(entities);
                return Result<IEnumerable<LeaveRequestDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<LeaveRequestDTO>>.Failure(
                    "An error occurred while retrieving leave requests.",
                    "INTERNAL_ERROR");
            }
        }
        public async Task<Result<LeaveRequestDTO>> CreateAsync(LeaveRequestCreateDTO dto)
        {
            try
            {
                // 1. Validaciones de negocio
                var businessValidation = await _businessValidator.ValidateCreateAsync(dto);
                if (!businessValidation.IsSuccess)
                {
                    return Result<LeaveRequestDTO>.Failure(businessValidation.ErrorMessage, businessValidation.ErrorCode);
                }

                // 2. transformar el DTO en una entidad
                var request = _mapper.Map<LeaveRequest>(dto);
                
                // 3. asignar el estado inicial
                request.Status = LeaveStatus.Pending;

                // 4. guardar en la base de datos
                _context.LeaveRequest.Add(request);
                await _context.SaveChangesAsync();

                // 5. cargar el empleado para incluir el nombre en la respuesta
                await _context.Entry(request)
                    .Reference(lr => lr.Employee)
                    .LoadAsync();

                // 6. retornar el DTO resultante
                var resultDto = _mapper.Map<LeaveRequestDTO>(request);
                return Result<LeaveRequestDTO>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return Result<LeaveRequestDTO>.Failure(
                    "An error occurred while creating the leave request.",
                    "INTERNAL_ERROR");
            }
        }

        public async Task<Result<LeaveRequestDTO>> UpdateStatusAsync(int id, LeaveRequestUpdateStatusDTO dto, EmployeeRole userRole)
        {
            try
            {
                // 1. Validaciones de negocio (incluyendo validación de rol)
                var businessValidation = await _businessValidator.ValidateUpdateStatusAsync(id, dto, userRole);
                if (!businessValidation.IsSuccess)
                {
                    return Result<LeaveRequestDTO>.Failure(businessValidation.ErrorMessage, businessValidation.ErrorCode);
                }

                // 2. buscar el leave request existente (ya validamos que existe)
                var existingRequest = await _context.LeaveRequest
                    .Include(lr => lr.Employee)
                    .FirstOrDefaultAsync(lr => lr.Id == id);

                // 3. actualizar solo el status
                existingRequest!.Status = dto.Status;

                // 4. guardar los cambios
                _context.LeaveRequest.Update(existingRequest);
                await _context.SaveChangesAsync();

                // 5. retornar el DTO actualizado
                var resultDto = _mapper.Map<LeaveRequestDTO>(existingRequest);
                return Result<LeaveRequestDTO>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return Result<LeaveRequestDTO>.Failure(
                    "An error occurred while updating the leave request status.",
                    "INTERNAL_ERROR");
            }
        }

        public async Task<Result> DeleteAsync(int id)
        {
            try
            {
                // 1. Validaciones de negocio
                var businessValidation = await _businessValidator.ValidateDeleteAsync(id);
                if (!businessValidation.IsSuccess)
                {
                    return Result.Failure(businessValidation.ErrorMessage, businessValidation.ErrorCode);
                }

                // 2. buscar el leave request a eliminar (ya validamos que existe)
                var leaveRequest = await _context.LeaveRequest.FindAsync(id);

                // 3. eliminar el registro
                _context.LeaveRequest.Remove(leaveRequest!);
                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    "An error occurred while deleting the leave request.",
                    "INTERNAL_ERROR");
            }
        }

    }
}
