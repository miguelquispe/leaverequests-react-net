using LeaveRequestAPI.Application.Common;

namespace LeaveRequestAPI.Application.Extensions;

public static class HttpContextExtensions
{
    public static AuthenticatedUser? GetAuthenticatedUser(this HttpContext httpContext)
    {
        return httpContext.Items["AuthenticatedUser"] as AuthenticatedUser;
    }
}
