using LGMDomains.Common;
using LGMDomains.Common.Helpers;
using LGMDomains.Identity;
using LGMPulse.Connections;
using LGMPulse.Connections.Helpers;
using LGMPulse.Domain.Domains;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace LGMPulse.WebApp.Controllers;

public class LGMController : Controller
{
    protected const string TEMPDATA_MENSAGEM = "Mensagem";
    protected const string TEMPDATA_AVISO = "Aviso";
    protected const string TEMPDATA_ERRO = "Erro";

    private SessionHelper? _sessionHelper;
    private WebAPIHelper? _webAPIHelper;

    protected SessionHelper SessionHelper =>
        _sessionHelper ??= HttpContext.RequestServices.GetRequiredService<SessionHelper>();
    protected WebAPIHelper WebAPIHelper =>
        _webAPIHelper ??= HttpContext.RequestServices.GetRequiredService<WebAPIHelper>();

    protected async Task<IActionResult> ValidateSessionAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            var valida = await validarSessao();
            return valida ?? await action();
        }
        catch (UnauthorizedAccessException)
        {
            var controller = RouteData.Values["controller"]?.ToString();
            var callAction = RouteData.Values["action"]?.ToString();

            if (controller?.Equals("Home", StringComparison.OrdinalIgnoreCase) == true &&
                callAction?.Equals("Index", StringComparison.OrdinalIgnoreCase) == true)
            {
                // Já estamos em Home/Index → evita loop
                return ViewError("Acesso não autorizado.");
            }

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            return ViewError(ex.Message);
        }
    }

    protected async Task<IActionResult> ExecuteViewAsync<TSource>(
        Func<Task<LGMResult<TSource>>>? serviceMethod = null,
        string? viewName = null) where TSource : new()
    {
        try
        {
            if (serviceMethod == null)
                return string.IsNullOrEmpty(viewName) ? View() : View(viewName);

            var resultService = await serviceMethod();
            if (!resultService.IsSuccess)
                return ViewError(resultService.Message); // GravarErro(resultService.Message ?? "Falha geral no serviço");
            else if (!string.IsNullOrEmpty(resultService.Message))
                GravarMensagem(resultService.Message);

            var model = resultService.Data ?? new TSource();
            return string.IsNullOrEmpty(viewName) ? View(model) : View(viewName, model);
        }
        catch (UnauthorizedAccessException)
        {
            var controller = RouteData.Values["controller"]?.ToString();
            var action = RouteData.Values["action"]?.ToString();

            if (controller?.Equals("Home", StringComparison.OrdinalIgnoreCase) == true &&
                action?.Equals("Index", StringComparison.OrdinalIgnoreCase) == true)
            {
                // Já estamos em Home/Index → evita loop
                return ViewError("Acesso não autorizado.");
            }

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            return ViewError(ex.Message);
        }
    }

    protected async Task<IActionResult> ExecuteViewAsync(
        Func<Task<ILGMResult>>? serviceMethod = null,
        string? viewName = null)
    {
        try
        {
            if (serviceMethod == null)
                return string.IsNullOrEmpty(viewName) ? View() : View(viewName);

            var resultService = await serviceMethod();
            if (!resultService.IsSuccess)
                return ViewError(resultService.Message); // GravarErro(resultService.Message ?? "Falha geral no serviço");
            else if (!string.IsNullOrEmpty(resultService.Message))
                GravarMensagem(resultService.Message);

            return string.IsNullOrEmpty(viewName) ? View() : View(viewName);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            return ViewError(ex.Message);
        }
    }

    private async Task<IActionResult?> validarSessao()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
        LGMSession? lgmSession = SessionHelper.GetLGMSession();

        if (lgmSession == null)
        {
            var lgmRefresh = SessionHelper.GetLGMRefresh();
            if (lgmRefresh == null || lgmRefresh.ExpireDateTime < DateTimeHelper.Now())
                return Redirect("/Home/Login");
            var refreshResult = await PostApiRefresh(lgmRefresh);
            if (!refreshResult || !GenerateCookie(lgmRefresh))
                return Redirect("/Home/Login");
            return null;
        }

        if (lgmSession.ExpireDateTime < DateTimeHelper.Now() ||
            lgmSession.User == null ||
            string.IsNullOrEmpty(lgmSession.User.UserLogin))
            return Redirect("/Home/Login");

        ViewBag.Session = lgmSession;
        
        //LGMSession session = new()
        //{
        //    ExpireDateTime = DateTime.Now.AddDays(1),
        //    User = new LocalUser()
        //    {
        //        DBKey = "lgm",
        //        ID = 1,
        //        UserEmail = "xanderib@gmail.com",
        //        UserLogin = "xanderib@gmail.com",
        //        UserName = "Alexandre",
        //        Token = "token_mock"
        //    }
        //};
        //ViewBag.Session = session;
        return null;
    }

    protected async Task PostApiLogout(LGMSession lgmSession)
    {
        var urlLogin = WebAPIHelper.GetAPIBaseUrl() + "/auth/v1/user/logout";
        try
        {
            using (var httpClient = WebAPIHelper.CreateHttpClient())
            {
                LoginModel loginModel = new()
                {
                    CodAplicacao = "pulse",
                    CodEmpresa = "",
                    Login = lgmSession.User.UserEmail,
                    Senha = string.Empty,
                };

                var response = await httpClient.PostAsJsonAsync(urlLogin, loginModel);
            }
        }
        catch (UnauthorizedAccessException)
        {
            Response.Cookies.Delete(ConnectionSettings.Instance.LGM_SESSION);
            Response.Cookies.Delete(ConnectionSettings.Instance.LGM_REFRESH);
        }
        catch (Exception) { }
    }

    private async Task<bool> PostApiRefresh(LGMSession lgmRefresh)
    {
        var urlLogin = WebAPIHelper.GetAPIBaseUrl() + "/auth/v1/refresh";

        using (var httpClient = new HttpClient())
        {
            var token = lgmRefresh.User.Token;
            if (string.IsNullOrEmpty(token)) return false;
            RefreshRequest refreshRequest = new() { RefreshToken = token };

            var response = await httpClient.PostAsJsonAsync(urlLogin, refreshRequest);
            if (!response.IsSuccessStatusCode)
                return false;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = await response.Content.ReadFromJsonAsync<LGMResult<string>>(options);
            return result != null && result.IsSuccess;
        }
    }

    protected bool GenerateCookie(LGMSession lgmSession)
    {
        if (!ValidateToken(lgmSession, out DateTime expiration))
            return false;  // Token inválido ou expirado

        var cookieValue = JsonSerializer.Serialize(lgmSession);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None, // SameSiteMode.Lax,
            Expires = DateTimeHelper.Now().AddMinutes(15)  // expiração curta do cookie para forçar validação do Token
        };

        Response.Cookies.Append(ConnectionSettings.Instance.LGM_SESSION, cookieValue, cookieOptions);

        //Cookie LGMRefresh
        cookieOptions.Expires = expiration;
        Response.Cookies.Append(ConnectionSettings.Instance.LGM_REFRESH, cookieValue, cookieOptions);
        return true;
    }

    public bool ValidateToken(LGMSession lgmSession, out DateTime expireDateTime)
    {
        expireDateTime = DateTimeHelper.Now();
        var token = lgmSession.User.Token ?? "";
        var tokenParts = token.Replace("Bearer ", "").Split('|');
        if (tokenParts.Length != 5) // userLogin|codCompany|codApp|expiration|signature
            return false;

        var userLogin = tokenParts[0];
        var codCompany = tokenParts[1];
        var codApp = tokenParts[2];
        var expirationStr = tokenParts[3];
        string signature = tokenParts[4];

        if (!DateTime.TryParse(expirationStr, out DateTime expiration) || expiration < expireDateTime)
            return false; // Token expirado

        expireDateTime = expiration;
        return true;
    }



    // Tratar mensagens de erros e avisos
    protected void GravarMensagem(string texto)
    {
        TempData[TEMPDATA_MENSAGEM] = texto;
    }

    protected void GravarAviso(string texto)
    {
        TempData[TEMPDATA_AVISO] = texto;
    }

    protected void GravarErro(string texto)
    {
        TempData[TEMPDATA_ERRO] = texto;
    }
    // Como aplicar na view.cshtml:
    //@if (TempData["Mensagem"] is not null)
    //{
    //    var msg = (dynamic)TempData["Mensagem"];
    //    <div class="alert @msg.Tipo">@msg.Texto</div>
    //}

    protected IActionResult ViewError(string? message)
    {
        ViewBag.Mensagem = message;
        return View("ViewError");
    }



}

