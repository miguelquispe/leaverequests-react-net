using LeaveRequestAPI.Application.Common;
using LeaveRequestAPI.Domain.Enums;
using LeaveRequestAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveRequestAPI.Application.Services;

public interface IAuthenticationService
{
    Task<Result<AuthenticatedUser>> ValidateUserAsync(int userId, string userRole);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly AppDbContext _context;

    public AuthenticationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AuthenticatedUser>> ValidateUserAsync(int userId, string userRole)
    {
        try
        {
            // 1. Buscar el usuario en la base de datos
            var user = await _context.Employee
                .FirstOrDefaultAsync(e => e.Id == userId);

            if (user == null)
            {
                return Result<AuthenticatedUser>.Failure(
                    $"User with ID {userId} does not exist.",
                    BusinessErrorCodes.USER_NOT_FOUND);
            }

            // 2. Validar que el rol del header coincida con el rol en BD
            string expectedRole = user.Role.ToString();
            if (!string.Equals(userRole, expectedRole, StringComparison.OrdinalIgnoreCase))
            {
                return Result<AuthenticatedUser>.Failure(
                    $"User role mismatch. Expected: {expectedRole}, but received: {userRole}.",
                    BusinessErrorCodes.INVALID_USER_ROLE);
            }

            // 3. Crear el usuario autenticado
            var authenticatedUser = new AuthenticatedUser
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return Result<AuthenticatedUser>.Success(authenticatedUser);
        }
        catch (Exception ex)
        {
            return Result<AuthenticatedUser>.Failure(
                "An error occurred during user authentication.",
                "INTERNAL_ERROR");
        }
    }
}
