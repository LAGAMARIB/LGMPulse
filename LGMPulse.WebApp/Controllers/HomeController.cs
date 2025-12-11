using LGMDomains.Common;
using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LGMPulse.WebApp.Controllers 
{
    public class HomeController : LGMController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => NewHealthyDashViewModel(), "Index")
            );
        }

        private async Task<LGMResult<HealthyDashViewModel>> NewHealthyDashViewModel()
        {
            HealthyDashViewModel viewModel = new()
            {
                TotalDespesas = 15360.00m,
                TotalReceitas = 6897.85m,
                PercDiferenca = -12.82m
            };
            return LGMResult.Ok(viewModel);
        }
    }
}
