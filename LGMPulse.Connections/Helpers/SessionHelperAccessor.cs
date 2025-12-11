using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LGMPulse.Connections.Helpers;

public static class SessionHelperAccessor
{
    public static SessionHelper Current =>
        new HttpContextAccessor().HttpContext!.RequestServices.GetRequiredService<SessionHelper>();
}
