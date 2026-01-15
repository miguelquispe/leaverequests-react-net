using FluentValidation.Results;
using LeaveRequestAPI.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace LeaveRequestAPI.Application.Extensions;

/// <summary>
/// Extension methods para los Controllers para facilitar el uso de ApiResponse
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Crear respuesta exitosa con datos
    /// </summary>
    public static ActionResult<ApiResponse<T>> ApiSuccess<T>(this ControllerBase controller, T data, int statusCode = 200)
    {
        var meta = CreateMeta(controller);
        var response = ApiResponse<T>.CreateSuccess(data, statusCode, meta);
        
        return controller.StatusCode(statusCode, response);
    }

    /// <summary>
    /// Crear respuesta exitosa sin datos (para NoContent, etc.)
    /// </summary>
    public static ActionResult<ApiResponse> ApiSuccess(this ControllerBase controller, int statusCode = 200)
    {
        var meta = CreateMeta(controller);
        var response = ApiResponse.CreateSuccess(statusCode, meta);
        
        return controller.StatusCode(statusCode, response);
    }

    /// <summary>
    /// Crear respuesta de error
    /// </summary>
    public static ActionResult<ApiResponse> ApiError(this ControllerBase controller, string message, string? code = null, int statusCode = 400)
    {
        var meta = CreateMeta(controller);
        var response = ApiResponse.CreateError(message, code, statusCode, meta);
        
        return controller.StatusCode(statusCode, response);
    }

    /// <summary>
    /// Crear respuesta de error con datos
    /// </summary>
    public static ActionResult<ApiResponse<T>> ApiError<T>(this ControllerBase controller, string message, string? code = null, int statusCode = 400)
    {
        var meta = CreateMeta(controller);
        var response = ApiResponse<T>.CreateError(message, code, statusCode, meta);
        
        return controller.StatusCode(statusCode, response);
    }

    /// <summary>
    /// Crear respuesta de errores de validación
    /// </summary>
    public static ActionResult<ApiResponse> ApiValidationError(this ControllerBase controller, ValidationResult validationResult, int statusCode = 400)
    {
        var validationDetails = validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );

        var meta = CreateMeta(controller);
        var response = ApiResponse.CreateError("Validation failed", "VALIDATION_ERROR", statusCode, meta, validationDetails);
        
        return controller.StatusCode(statusCode, response);
    }

    /// <summary>
    /// Crear respuesta de errores de validación con datos tipados
    /// </summary>
    public static ActionResult<ApiResponse<T>> ApiValidationError<T>(this ControllerBase controller, ValidationResult validationResult, int statusCode = 400)
    {
        var validationDetails = validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );

        var meta = CreateMeta(controller);
        var response = ApiResponse<T>.CreateError("Validation failed", "VALIDATION_ERROR", statusCode, meta, validationDetails);
        
        return controller.StatusCode(statusCode, response);
    }

    /// <summary>
    /// Crear respuesta de errores de validación de ModelState
    /// </summary>
    public static ActionResult<ApiResponse> ApiValidationError(this ControllerBase controller, int statusCode = 400)
    {
        var validationDetails = controller.ModelState
            .Where(x => x.Value != null && x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var meta = CreateMeta(controller);
        var response = ApiResponse.CreateError("Validation failed", "VALIDATION_ERROR", statusCode, meta, validationDetails);
        
        return controller.StatusCode(statusCode, response);
    }

    /// <summary>
    /// Crear respuesta de errores de validación de ModelState con datos tipados
    /// </summary>
    public static ActionResult<ApiResponse<T>> ApiValidationError<T>(this ControllerBase controller, int statusCode = 400)
    {
        var validationDetails = controller.ModelState
            .Where(x => x.Value != null && x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var meta = CreateMeta(controller);
        var response = ApiResponse<T>.CreateError("Validation failed", "VALIDATION_ERROR", statusCode, meta, validationDetails);
        
        return controller.StatusCode(statusCode, response);
    }

    /// <summary>
    /// Crear respuesta desde Result con datos
    /// </summary>
    public static ActionResult<ApiResponse<T>> ApiFromResult<T>(this ControllerBase controller, Result<T> result, int successStatus = 200)
    {
        var meta = CreateMeta(controller);
        
        if (result.IsSuccess)
        {
            var response = ApiResponse<T>.CreateSuccess(result.Data!, successStatus, meta);
            return controller.StatusCode(successStatus, response);
        }

        // Mapear códigos de error a códigos HTTP
        var httpStatus = MapErrorCodeToHttpStatus(result.ErrorCode);
        var errorResponse = ApiResponse<T>.CreateError(result.ErrorMessage, result.ErrorCode, httpStatus, meta);
        
        return controller.StatusCode(httpStatus, errorResponse);
    }

    /// <summary>
    /// Crear respuesta desde Result sin datos
    /// </summary>
    public static ActionResult<ApiResponse> ApiFromResult(this ControllerBase controller, Result result, int successStatus = 200)
    {
        var meta = CreateMeta(controller);
        
        if (result.IsSuccess)
        {
            var response = ApiResponse.CreateSuccess(successStatus, meta);
            return controller.StatusCode(successStatus, response);
        }

        // Mapear códigos de error a códigos HTTP
        var httpStatus = MapErrorCodeToHttpStatus(result.ErrorCode);
        var errorResponse = ApiResponse.CreateError(result.ErrorMessage, result.ErrorCode, httpStatus, meta);
        
        return controller.StatusCode(httpStatus, errorResponse);
    }

    /// <summary>
    /// Crear metainformación desde el contexto del controller
    /// </summary>
    private static MetaInfo CreateMeta(ControllerBase controller)
    {
        var requestId = controller.HttpContext.TraceIdentifier;
        
        return new MetaInfo
        {
            RequestId = requestId,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Mapear códigos de error de negocio a códigos HTTP apropiados
    /// </summary>
    private static int MapErrorCodeToHttpStatus(string errorCode)
    {
        return errorCode switch
        {
            BusinessErrorCodes.REQUEST_NOT_FOUND => 404,
            BusinessErrorCodes.EMPLOYEE_NOT_FOUND => 404,
            BusinessErrorCodes.USER_NOT_FOUND => 404,
            
            BusinessErrorCodes.INVALID_STATUS_TRANSITION => 400,
            BusinessErrorCodes.START_DATE_IN_PAST => 400,
            BusinessErrorCodes.INVALID_DATE_RANGE => 400,
            BusinessErrorCodes.OVERLAPPING_REQUEST => 400,
            BusinessErrorCodes.CANNOT_DELETE_APPROVED_REQUEST => 400,
            BusinessErrorCodes.INVALID_USER_ROLE => 400,
            BusinessErrorCodes.MISSING_AUTH_HEADERS => 400,
            
            BusinessErrorCodes.OPERATION_NOT_ALLOWED => 403,
            BusinessErrorCodes.INSUFFICIENT_PERMISSIONS => 403,
            
            _ => 500  // Error interno por defecto
        };
    }
}