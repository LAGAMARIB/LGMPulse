using LGMDomains.Common;
using LGMDomains.Common.Helpers;
using LGMDomains.Identity;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;
using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace LGMPulse.WebApp.Controllers 
{
    public class HomeController : LGMController
    {
        private readonly IMovtoService _movtoService;
        private readonly ILoginService? _loginService;

        public HomeController(IMovtoService movtoService, ILoginService? loginService)
        {
            _movtoService = movtoService;
            _loginService = loginService;
        }

        public async Task<IActionResult> Index(int? mes, int? ano)
        {
            return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => NewHealthyDashViewModel(ano, mes), "Index")
            );
        }

        private async Task<LGMResult<HealthyDashViewModel>> NewHealthyDashViewModel(int? year=null, int? month=null)
        {
            year ??= DateTimeHelper.Now().Year;
            month ??= DateTimeHelper.Now().Month;
            var result = await _movtoService.GetListAsync(year.Value, month.Value);
            var culture = new CultureInfo("pt-BR");
            HealthyDashViewModel viewModel = new()
            {
                Ano = year.Value,
                Mes = month.Value,
                IsMesAtual = (year == DateTimeHelper.Now().Year && month == DateTimeHelper.Now().Month),
                MesReferencia = culture.DateTimeFormat.GetMonthName(month.Value) + " / " + year.ToString()
            };
            foreach (var movto in result.Data!)
            {
                if (movto.TipoMovto == TipoMovtoEnum.Despesa)
                    viewModel.TotalDespesas += movto.ValorMovto ?? 0;
                else
                    viewModel.TotalReceitas += movto.ValorMovto ?? 0;
            }
            //viewModel.PercDiferenca = CalcDifLiquidez();
            return LGMResult.Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            SessionHelper.ClearCookies(Request, Response);
            return View(new LoginViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            LoginModel loginModel = new()
            {
                CodAplicacao = "pfinan",
                CodEmpresa = "",
                Login = model.Email,
                Senha = model.Senha
            };
            LGMResult<LGMUser> result = await _loginService!.ValidateLoginAsync(loginModel);
            if (!result.IsSuccess || result.Data == null)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Falha na autenticação");
                return View(model);
            }
            LGMUser lgmUser = result.Data;

            if (lgmUser.RegisterStatus == UserRegisterStatus.New || lgmUser.RegisterStatus == UserRegisterStatus.Initializing)
            {
                ModelState.AddModelError(string.Empty, "Sua conta está sendo preparada. Aguarde email de ativação.");
                return View(model);
            }

            if (lgmUser.RegisterStatus != UserRegisterStatus.Ready)
            {
                ModelState.AddModelError(string.Empty, "Usuário não habilitado ou conta desativada");
                return View(model);
            }

            LGMResult<LocalUser> localUserResult = await _loginService.GetLocalUser(lgmUser);
            if (!localUserResult.IsSuccess || localUserResult.Data == null)
            {
                ModelState.AddModelError(string.Empty, localUserResult.Message ?? "Falha na autenticação - usuário local não ativado");
                return View(model);
            }

            LocalUser localUser = localUserResult.Data;  // DBKey n UserEmail assigned
            localUser.UserLogin = lgmUser.UserLogin;
            localUser.UserName = lgmUser.UserName;
            localUser.Token = lgmUser.Token;
            LGMSession lgmSession = new LGMSession()
            {
                User = localUser
            };

            if (!this.GenerateCookies(lgmSession))
            {
                ModelState.AddModelError(string.Empty, "Falha na autenticação do usuário");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> LogoutAsync()  
        {
            LGMSession? lgmSession = SessionHelper.GetLGMSession_Cookie() ?? SessionHelper.GetLGMRefresh_Cookie();
            if (lgmSession != null)
            {
                try
                {
                    await PostApiLogout(lgmSession);
                }
                catch (Exception)
                {
                    //ignorar exceção
                }
            }
            SessionHelper.ClearCookies(Request, Response);
            return View("Login", new LoginViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> AlterarSenha()
        {
            return await ValidateSessionAsync(() => ExecuteViewAsync<object>());
        }

        [HttpPost]
        public async Task<JsonResult> AlterarSenhaAsync(LoginModel model)
        {
            LGMResult<string> result;
            try
            {
                result = await _loginService!.ChangePasswordAsync(model);
                if (result.IsSuccess)
                {
                    result.RedirectUrl = "/home/login";
                    GravarMensagem(result.Message ?? "Senha alterada com sucesso");
                }
            }
            catch (UnauthorizedAccessException)
            {
                var mens = "Sessão expirada. Faça login novamente.";
                result = LGMResult.Fail<string>(mens, "/home/login");
                GravarMensagem(mens);
            }
            return Json(result);
        }

        [HttpGet]
        public IActionResult CriarContaUsuarioAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CriarContaUsuarioAsync(RequestRegisterModel model)
        {
            LGMResult<LGMUser> result = await _loginService!.CreateUser(model);
            if (result.IsSuccess)
            {
                var user = result.Data;
                result.RedirectUrl = "/home/login";
                result.Message = "Cadastro recebido! Você receberá um e-mail quando sua conta for ativada.";
                GravarMensagem(result.Message);
            }
            return Json(result);
        }


    }
}
