using Microsoft.AspNetCore.Mvc;

namespace LGMPulse.WebApp.Controllers;

public class LancamentoController : Controller
{
    public IActionResult NovaReceita()
    {
        return View();
    }

    public IActionResult NovaDespesa()
    {
        return View();
    }

    public IActionResult DigitarValor()
    {
        return View();
    }
}
