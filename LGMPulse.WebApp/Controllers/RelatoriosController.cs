using Microsoft.AspNetCore.Mvc;

namespace LGMPulse.WebApp.Controllers;

public class RelatoriosController : Controller
{
    [HttpGet("relatorios")]
    public IActionResult Relatorios()
    {
        return View();
    }
}
