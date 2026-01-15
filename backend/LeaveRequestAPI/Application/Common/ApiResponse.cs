using System.Text.Json.Serialization;

namespace LeaveRequestAPI.Application.Common;

/// <summary>
/// Respuesta estándar de la API
/// </summary>
/// <typeparam name="T">Tipo de datos de la respuesta</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indica si la operación fue exitosa
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Código de estado HTTP
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Datos de la respuesta (cuando es exitosa)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }

    /// <summary>
    /// Información del error (cuando no es exitosa)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ErrorInfo? Errors { get; set; }

    /// <summary>
    /// Metainformación adicional
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MetaInfo? Meta { get; set; }

    public ApiResponse()
    {
    }

    public ApiResponse(bool success, int status, T? data = default, ErrorInfo? errors = null, MetaInfo? meta = null)
    {
        Success = success;
        Status = status;
        Data = data;
        Errors = errors;
        Meta = meta;
    }

    /// <summary>
    /// Crear respuesta exitosa
    /// </summary>
    public static ApiResponse<T> CreateSuccess(T data, int status = 200, MetaInfo? meta = null)
    {
        return new ApiResponse<T>(true, status, data, null, meta);
    }

    /// <summary>
    /// Crear respuesta de error
    /// </summary>
    public static ApiResponse<T> CreateError(string message, string? code = null, int status = 400, MetaInfo? meta = null, Dictionary<string, string[]>? validationDetails = null)
    {
        var errors = new ErrorInfo
        {
            Code = code,
            Message = message,
            Details = validationDetails
        };

        return new ApiResponse<T>(false, status, default, errors, meta);
    }

    /// <summary>
    /// Crear respuesta de error desde Result
    /// </summary>
    public static ApiResponse<T> CreateFromResult(Result<T> result, int successStatus = 200, int errorStatus = 400, MetaInfo? meta = null)
    {
        if (result.IsSuccess)
        {
            return CreateSuccess(result.Data!, successStatus, meta);
        }

        return CreateError(result.ErrorMessage, result.ErrorCode, errorStatus, meta);
    }
}

/// <summary>
/// Versión sin datos para respuestas que no retornan contenido
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public ApiResponse() : base() { }

    public ApiResponse(bool success, int status, ErrorInfo? errors = null, MetaInfo? meta = null) 
        : base(success, status, null, errors, meta)
    {
    }

    /// <summary>
    /// Crear respuesta exitosa sin datos
    /// </summary>
    public static ApiResponse CreateSuccess(int status = 200, MetaInfo? meta = null)
    {
        return new ApiResponse(true, status, null, meta);
    }

    /// <summary>
    /// Crear respuesta de error sin datos
    /// </summary>
    public static new ApiResponse CreateError(string message, string? code = null, int status = 400, MetaInfo? meta = null, Dictionary<string, string[]>? validationDetails = null)
    {
        var errors = new ErrorInfo
        {
            Code = code,
            Message = message,
            Details = validationDetails
        };

        return new ApiResponse(false, status, errors, meta);
    }

    /// <summary>
    /// Crear respuesta desde Result sin datos
    /// </summary>
    public static ApiResponse CreateFromResult(Result result, int successStatus = 200, int errorStatus = 400, MetaInfo? meta = null)
    {
        if (result.IsSuccess)
        {
            return CreateSuccess(successStatus, meta);
        }

        return CreateError(result.ErrorMessage, result.ErrorCode, errorStatus, meta);
    }
}

/// <summary>
/// Información del error
/// </summary>
public class ErrorInfo
{
    /// <summary>
    /// Código del error
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code { get; set; }

    /// <summary>
    /// Mensaje del error
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    /// <summary>
    /// Detalles adicionales del error (ej: errores de validación por campo)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? Details { get; set; }
}

/// <summary>
/// Metainformación adicional
/// </summary>
public class MetaInfo
{
    /// <summary>
    /// Timestamp de la respuesta
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID único de la petición para tracking
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RequestId { get; set; }

    /// <summary>
    /// Información adicional específica del endpoint
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Additional { get; set; }
}