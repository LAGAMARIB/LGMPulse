using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace LGMPulse.Connections.Helpers;

public class WebAPIHelper
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SessionHelper _sessionHelper;

    public WebAPIHelper(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _sessionHelper = sessionHelper;
    }

    public string GetAPIBaseUrl()
    {
        //return "http://127.0.0.1:5003";
        //return "https://api.lagama.com.br";
        return _configuration["AppParameters:WebAPIBaseUrl"]
               ?? throw new InvalidOperationException("AppParameters:WebAPIBaseUrl não encontrado.");

    }

    public HttpClient CreateHttpClient(string? dbKey = null)
    {
        var session = _sessionHelper.GetLGMSession();
        if (session == null)
        {
            throw new UnauthorizedAccessException("CreateHttpClient: Sessão expirada. Faça login novamente."); 
        }

        var _user = session.User ?? throw new UnauthorizedAccessException("CreateHttpClient: Usuário não autenticado.");
        var token = _user.Token ?? throw new UnauthorizedAccessException("CreateHttpClient: Token não encontrado.");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("X-DbKey", dbKey ?? _user.DBKey);

        return client;
    }

}
