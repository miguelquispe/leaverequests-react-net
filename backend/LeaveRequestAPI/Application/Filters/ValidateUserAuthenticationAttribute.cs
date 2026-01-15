using LeaveRequestAPI.Application.Common;
using LeaveRequestAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeaveRequestAPI.Application.Filters;

public class ValidateUserAuthenticationAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ValidateUserAuthenticationAttribute>>();
        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

        // 1. Validar y extraer X-User-Id
        if (!context.HttpContext.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) ||
            string.IsNullOrEmpty(userIdHeader) ||
            !int.TryParse(userIdHeader, out int userId) ||
            userId <= 0)
        {
            logger.LogWarning("Missing or invalid X-User-Id header");
            context.Result = new BadRequestObjectResult(new
            {
                message = "Se requiere un header X-User-Id válido",
                code = BusinessErrorCodes.MISSING_AUTH_HEADERS
            });
            return;
        }

        // 2. Validar y extraer X-User-Role
        if (!context.HttpContext.Request.Headers.TryGetValue("X-User-Role", out var userRoleHeader) ||
            string.IsNullOrEmpty(userRoleHeader))
        {
            logger.LogWarning("Missing X-User-Role header");
            context.Result = new BadRequestObjectResult(new
            {
                message = "Se requiere el header X-User-Role",
                code = BusinessErrorCodes.MISSING_AUTH_HEADERS
            });
            return;
        }

        string userRole = userRoleHeader.ToString();

        // 3. Validar usuario contra la base de datos
        var authResult = await authService.ValidateUserAsync(userId, userRole);
        if (!authResult.IsSuccess)
        {
            logger.LogWarning("Authentication failed for user {UserId}: {ErrorMessage}", userId, authResult.ErrorMessage);
            
            var statusCode = authResult.ErrorCode switch
            {
                BusinessErrorCodes.USER_NOT_FOUND => 404,
                BusinessErrorCodes.INVALID_USER_ROLE => 403,
                _ => 500
            };

            context.Result = new ObjectResult(new
            {
                message = authResult.ErrorMessage,
                code = authResult.ErrorCode
            })
            {
                StatusCode = statusCode
            };
            return;
        }

        // 4. Agregar el usuario autenticado al contexto para uso en el controller
        context.HttpContext.Items["AuthenticatedUser"] = authResult.Data;

        logger.LogInformation("User {UserId} authenticated successfully with role {UserRole}", userId, userRole);

        // 5. Continuar con la ejecución del action
        await next();
    }
}
