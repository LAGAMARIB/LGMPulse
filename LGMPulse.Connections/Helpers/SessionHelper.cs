using LGMPulse.Domain.Domains;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace LGMPulse.Connections.Helpers;

public class SessionHelper
{
    private IHttpContextAccessor? _accessor;

    public SessionHelper(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public LGMSession? GetLGMSession()
    {
        var cookie = _accessor?.HttpContext?.Request.Cookies[ConnectionSettings.Instance.LGM_SESSION];
        if (string.IsNullOrEmpty(cookie))
            return null;

        try
        {
            return JsonSerializer.Deserialize<LGMSession>(cookie);
        }
        catch
        {
            return null;
        }
    }

    public LGMSession? GetLGMRefresh()
    {
        var cookie = _accessor?.HttpContext?.Request.Cookies[ConnectionSettings.Instance.LGM_REFRESH];
        if (string.IsNullOrEmpty(cookie))
            return null;

        try
        {
            return JsonSerializer.Deserialize<LGMSession>(cookie);
        }
        catch
        {
            return null;
        }
    }

}
