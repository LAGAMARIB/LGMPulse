using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Enuns;
using Microsoft.AspNetCore.Mvc;

namespace LGMPulse.WebApp.Controllers;

public class RelatoriosController : LGMController
{
    private readonly IMovtoService _movtoService;

    public RelatoriosController(IMovtoService movtoService)
    {
        _movtoService = movtoService;
    }

    [HttpGet("relatorios")]
    public IActionResult Relatorios()
    {
        return View();
    }

    [HttpGet("relatorios/extrato/{ano}/{mes}/{tipoMovto=null}")]
    public async Task<IActionResult> ExtratoAsync(int ano, int mes, TipoMovtoEnum? tipoMovto=null)
    {
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync(() => _movtoService.GetListAsync(ano, mes, tipoMovto ) )
        );
    }
}
