using LGMDomains.Common;
using LGMDomains.Common.Helpers;
using LGMDomains.Identity;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Connections.Helpers;
using LGMPulse.Domain.Domains;
using LGMPulse.Persistence.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace LGMPulse.AppServices.Services;

internal class LoginService : ILoginService
{
    private readonly ILocalUserRepository _localUserRepository;
    private readonly WebAPIHelper _webAPIHelper;

    public LoginService(ILocalUserRepository localUserRepository, WebAPIHelper webAPIHelper)
    {
        _localUserRepository = localUserRepository;
        _webAPIHelper = webAPIHelper;
    }

    public async Task<LGMResult<LocalUser>> GetLocalUser(LGMUser lgmUser)
    {
        try
        {
            if (string.IsNullOrEmpty(lgmUser.UserEmail))
                throw new Exception("Email inválido. Usuário não autenticado.");
            var localUser = await _localUserRepository.GetByEmailAsync(lgmUser.UserEmail);
            if (localUser == null)
                throw new Exception("LoginService.GetLocalUser: Usuário não autenticado");
            return LGMResult.Ok(localUser);
        }
        catch (Exception ex)
        {
            return LGMResult.Fail<LocalUser>(ex.Message);
        }
    }

    public async Task<LGMResult<string>> RecoverPasswordAsync(LoginModel loginModel)
    {
        var urlLogin = _webAPIHelper.GetAPIBaseUrl() + "/auth/v1/user/restore-password";

        using (var httpClient = new HttpClient())
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("X-DbKey", "");

                var response = await httpClient.PostAsJsonAsync(urlLogin, loginModel);
                if (!response.IsSuccessStatusCode)
                    return LGMResult.Fail<string>($"Erro na API: {response.StatusCode}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                //var result = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadFromJsonAsync<LGMResult<string>>(options);
                if (result == null || !result.IsSuccess)
                    return LGMResult.Fail<string>(result?.Message ?? "Falha geral ao recuperar senha");

                return LGMResult.Ok<string>(result!.Message);
            }
            catch (Exception ex)
            {
                return LGMResult.Fail<string>($"Erro de execução da API: {ex.Message}");
            }
        }
    }

    public async Task<LGMResult<LGMUser>> ValidateLoginAsync(LoginModel loginModel)
    {
        var urlLogin = _webAPIHelper.GetAPIBaseUrl() + "/auth/v1/login";
        var requestBody = new
        {
            codAplicacao = "pulse",
            codEmpresa = "",
            login = loginModel.Login,
            senha = CryptoHelper.Encrypt(loginModel.Senha ?? "")
        };

        using (var httpClient = new HttpClient())
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("X-DbKey", "");

                var response = await httpClient.PostAsJsonAsync(urlLogin, requestBody);
                if (!response.IsSuccessStatusCode)
                    return LGMResult.Fail<LGMUser>($"Erro na API: {response.StatusCode}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadFromJsonAsync<LGMResult<LGMUser>>(options);
                return result ?? LGMResult.Fail<LGMUser>($"API retornou resposta inválida");
            }
            catch (Exception ex)
            {
                return LGMResult.Fail<LGMUser>($"Erro de execução da API: {ex.Message}");
            }
        }

    }

    public async Task<LGMResult<string>> ChangePasswordAsync(LoginModel model)
    {
        var urlLogin = _webAPIHelper.GetAPIBaseUrl() + "/auth/v1/user/change-password";

        try
        {
            using (var httpClient = _webAPIHelper.CreateHttpClient())
            {
                model.Senha = CryptoHelper.Encrypt(model.Senha!);

                var response = await httpClient.PostAsJsonAsync(urlLogin, model.Senha);
                if (!response.IsSuccessStatusCode)
                    return LGMResult.Fail<string>($"Erro na API: {response.StatusCode}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await response.Content.ReadFromJsonAsync<LGMResult<string>>(options);
                return result ?? LGMResult.Fail<string>($"API retornou resposta inválida");
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return LGMResult.Fail<string>($"Erro de execução da API: {ex.Message}");
        }
    }

    public async Task<LGMResult<LGMUser>> CreateUser(RequestRegisterModel model)
    {
        var urlCreate = _webAPIHelper.GetAPIBaseUrl() + "/auth/v1/user/request-register";

        if (model is null ||
            string.IsNullOrWhiteSpace(model.UserEmail) ||
            string.IsNullOrWhiteSpace(model.UserName))
            return LGMResult.Fail<LGMUser>("Campos obrigatórios em branco (Nome, Email, Celular)");

        using (var httpClient = new HttpClient())
        {
            try
            {
                model.CodApplication = "pulse";
                model.ApplicationName = "LAGAMA Pulse";
                model.CodCompany = "";
                model.UserLogin = model.UserEmail;

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var response = await httpClient.PostAsJsonAsync(urlCreate, model, options);
                if (!response.IsSuccessStatusCode)
                    return LGMResult.Fail<LGMUser>($"Erro na API: {response.StatusCode}");
                var result = await response.Content.ReadFromJsonAsync<LGMResult<LGMUser>>(options);
                return result ?? LGMResult.Fail<LGMUser>($"API retornou resposta inválida");
            }
            catch (Exception ex)
            {
                return LGMResult.Fail<LGMUser>($"Erro de execução da API: {ex.Message}");
            }
        }
    }
}
