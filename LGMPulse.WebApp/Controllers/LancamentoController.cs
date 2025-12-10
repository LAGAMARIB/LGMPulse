using LGMPulse.WebApp.Models;
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

    [HttpGet("Lancamento/DigitarValor/{tipo}/{idGrupo}/{descricao=null}")]
    public IActionResult DigitarValor(TipoMovtoEnum tipo, int idGrupo, string? descricao)
    {
        DigitarValorViewModel model = new()
        {
            TipoMovto = tipo,
            IDGrupo = idGrupo,
            DescGrupo = descricao ?? ""
        };
        return View(model);
    }
}
