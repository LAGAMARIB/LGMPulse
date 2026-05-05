using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LGMPulse.Connections.Helpers;

public class WebAPIHelper
{
    private readonly IConfiguration _configuration;
    private readonly SessionHelper _sessionHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WebAPIHelper(
        IConfiguration configuration, 
        SessionHelper sessionHelper,
        IHttpContextAccessor httpContextAccessor )
    {
        _configuration = configuration;
        _sessionHelper = sessionHelper;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetAPIBaseUrl()
    {
        return _configuration["AppParameters:WebAPIBaseUrl"]
               ?? throw new InvalidOperationException("AppParameters:WebAPIBaseUrl não encontrado.");

    }

    public HttpClient CreateHttpClient(string? dbKey = null)
    {
        var session = _sessionHelper.GetLGMSession_Cookie();
        if (session == null)
        {
            throw new UnauthorizedAccessException("CreateHttpClient: Sessão expirada. Faça login novamente."); 
        }

        var _user = session.User ?? throw new UnauthorizedAccessException("CreateHttpClient: Usuário não autenticado.");
        var token = _user.Token ?? throw new UnauthorizedAccessException("CreateHttpClient: Token não encontrado.");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("X-DbKey", dbKey ?? _user.DBKey);
        
        var userIp = _httpContextAccessor.HttpContext?
            .Connection?
            .RemoteIpAddress?
            .ToString();
        client.DefaultRequestHeaders.Remove("X-Client-IP");
        client.DefaultRequestHeaders.Add("X-Client-IP", userIp);

        return client;
    }

    public async Task<T?> SendPostAsync<T>(string relativeUrl, object? bodyRequestObj = null)
    {
        var url = GetAPIBaseUrl() + relativeUrl;

        using var httpClient = CreateHttpClient();

        try
        {
            var response = await httpClient.PostAsJsonAsync(url, bodyRequestObj);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro na API: {response.StatusCode}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result =
                await response.Content.ReadFromJsonAsync<T>(options);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao executar POST em {url}: {ex.Message}", ex);
        }
    }

    public async Task<T?> SendGetAsync<T>(string relativeUrl)
    {
        var url = GetAPIBaseUrl() + relativeUrl;

        using var httpClient = CreateHttpClient();

        try
        {
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro na API: {response.StatusCode}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return await response.Content.ReadFromJsonAsync<T>(options);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao executar GET em {url}: {ex.Message}", ex);
        }
    }
}
