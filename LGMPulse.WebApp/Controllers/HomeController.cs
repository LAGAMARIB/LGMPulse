using LGMDomains.Common;
using LGMDomains.Common.Helpers;
using LGMDomains.Identity;
using LGMPulse.AppServices.Helpers;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.ViewModels;
using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

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
                
        public async Task<IActionResult> Index(int? ano, int? mes)
        {
            return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => NewHealthyDashViewModel(ano, mes), "Index")
            );
        }

        private async Task<LGMResult<HealthyDashViewModel>> NewHealthyDashViewModel(int? year=null, int? month=null)
        {
            DateTime hoje = DateTimeHelper.Now();
            year ??= DateTimeHelper.Now().Year;
            month ??= DateTimeHelper.Now().Month;

            int anoAnterior = month.Value == 1 ? year.Value - 1 : year.Value;
            int mesAnterior = month.Value == 1 ? 12 : month.Value - 1;
            bool isMesAtual = year == hoje.Year && month == hoje.Month;

            LGMResult<SumarioMes> anteriorResult;
            LGMResult<SumarioMes> atualResult;
            if (isMesAtual)
            {
                anteriorResult = await _movtoService.GetSumarioAteAsync(anoAnterior, mesAnterior, hoje.Day);
                atualResult= await _movtoService.GetSumarioAteAsync(year.Value, month.Value, hoje.Day); 
            }
            else
            {
                anteriorResult = await _movtoService.GetSumarioMesAsync(anoAnterior, mesAnterior);
                atualResult = await _movtoService.GetSumarioMesAsync(year.Value, month.Value);
            }

            var sumarioAnterior = anteriorResult.Data;
            decimal liquidezAnterior = (sumarioAnterior != null ? sumarioAnterior.TotalReceitas - sumarioAnterior.TotalDespesas : 0);

            HealthyDashViewModel viewModel = new()
            {
                Year = year.Value,
                Month = month.Value
            };
            var sumario = atualResult.Data;
            viewModel.TotalReceitas = sumario?.TotalReceitas ?? 0;
            viewModel.TotalDespesas = sumario?.TotalDespesas ?? 0;
            viewModel.IsFreeMode = LocalUserHelper.GetLocalUser().SubscriptLevel == 0;
            viewModel.IsMesAtual = isMesAtual;

            decimal liquidezAtual = viewModel.TotalReceitas - viewModel.TotalDespesas;
            if (liquidezAnterior == 0)
            {
                viewModel.PercDiferenca = liquidezAtual == 0 ? 0 : liquidezAtual < 0 ? -100 : 100;
            }
            else
            {
                viewModel.PercDiferenca =
                    ((liquidezAtual - liquidezAnterior) / Math.Abs(liquidezAnterior)) * 100;
            }
            return LGMResult.Ok(viewModel);
        }

        [HttpGet]
        public IActionResult Login()
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

            LGMUser? lgmUser = result.Data;
            if (lgmUser != null &&
                lgmUser.SubscriptStatus == LGMDomains.Identity.Enums.SubscriptStatusEnum.Vencido &&
                lgmUser.IsOwner == true)
            {
                return RedirectToAction("RenovarAssinatura", new
                {
                    Name = lgmUser.UserName,
                    Email = lgmUser.UserEmail,
                    Phone = lgmUser.UserPhone,
                    CodCompany = lgmUser.CodCompany,
                    CompanyName = lgmUser.CompanyName
                });
            }

            if (!result.IsSuccess || result.Data == null)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Falha na autenticação");
                return View(model);
            }

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

            LGMSession lgmSession = new LGMSession()
            {
                User = lgmUser
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
        public IActionResult RecuperarSenha()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> RecuperarSenhaAsync(LoginViewModel model)
        {
            LoginModel loginModel = new()
            {
                CodAplicacao = "pulse",
                NomeAplicacao = "LAGAMA Pulse",
                UrlAplicacao = "https://pulse.lagama.com.br",
                CodEmpresa = "",
                Login = model.Email
            };

            LGMResult<string> result = await _loginService!.RecoverPasswordAsync(loginModel);
            if (result.IsSuccess)
            {
                result.RedirectUrl = $"/home/login";
                GravarMensagem(result.Message ?? "Email enviado");
            }

            return Json(result);
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

        [HttpGet]
        public IActionResult RenovarAssinatura(CheckoutRequest checkout)
        {
            return View(checkout);
        }

    }
}
