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

    public LGMSession? GetLGMSession_Cookie()
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

    public LGMSession? GetLGMRefresh_Cookie()
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

    public void ClearCookies(HttpRequest Request, HttpResponse Response)
    {
        if (Request.Cookies.ContainsKey(ConnectionSettings.Instance.LGM_SESSION))
            Response.Cookies.Delete(ConnectionSettings.Instance.LGM_SESSION);
        if (Request.Cookies.ContainsKey(ConnectionSettings.Instance.LGM_REFRESH))
            Response.Cookies.Delete(ConnectionSettings.Instance.LGM_REFRESH);
    }

    public T? ReadSession<T>(string contentKey) where T : class
    {
        var session = _accessor?.HttpContext?.Session;
        if (session == null)
            return default;

        var sessionValue = session.GetString(contentKey);
        if (string.IsNullOrEmpty(sessionValue))
            return default;

        if (typeof(T) == typeof(string) || typeof(T).IsPrimitive)
            return (T)(object)sessionValue;

        try
        {
            T? result = JsonSerializer.Deserialize<T>(sessionValue);
            return result;
        }
        catch (Exception)
        {
            return default;
        }
    }

    public void WriteSession<T>(string contentKey, T contentValue)
    {
        var session = _accessor?.HttpContext?.Session;
        if (session == null)
            return;

        string? sessionValue;

        if (contentValue is string || typeof(T).IsPrimitive)
        {
            sessionValue = contentValue?.ToString();
        }
        else
        {
            sessionValue = JsonSerializer.Serialize(contentValue);
        }

        session.SetString(contentKey, sessionValue ?? "");
    }

    public void RemoveSession(string contentKey)
    {
        var session = _accessor?.HttpContext?.Session;
        if (session == null)
            return;

        session.Remove(contentKey);
    }

}
