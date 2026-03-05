using LGMDomains.Identity;
using LGMPulse.Connections.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LGMPulse.AppServices.Helpers;

public class LocalUserHelper
{
    public static LGMUser GetLocalUser()
    {
        SessionHelper SessionHelper = new HttpContextAccessor().HttpContext!.RequestServices.GetRequiredService<SessionHelper>();
        var _user = SessionHelper.GetLGMSession_Cookie()?.User;
        if (_user == null)
            throw new UnauthorizedAccessException("LocalUserHelper: Usuário não autenticado.");

        return _user;
    }

    public static LGMUser? GetLocalUserOrDefault()
    {
        SessionHelper SessionHelper = new HttpContextAccessor().HttpContext!.RequestServices.GetRequiredService<SessionHelper>();
        return SessionHelper.GetLGMSession_Cookie()?.User;
    }
}
