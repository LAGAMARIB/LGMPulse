using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace LGMPulse.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HealthyDashViewModel viewModel = new()
            {
                TotalDespesas = 5360.00m,
                TotalReceitas = 6900.85m,
                PercDiferenca = 12.82m
            };
            return View(viewModel);
        }

    }
}
