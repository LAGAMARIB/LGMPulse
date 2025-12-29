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
    public async Task<IActionResult> Relatorios()
    {
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync()
        );
    }

    [HttpGet("relatorios/extrato/{ano}/{mes}/{tipoMovto=null}")]
    public async Task<IActionResult> ExtratoAsync(int ano, int mes, TipoMovtoEnum? tipoMovto=null)
    {
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync(() => _movtoService.GetListAsync(ano, mes, tipoMovto ) )
        );
    }

    [HttpGet("/relatorios/grupos/{ano=0}/{mes=0}")]
    public async Task<IActionResult> RelatorioGrupos(int ano=0, int mes=0)
    {

        return PartialView();
    }

    [HttpGet("/relatorios/evolucao/{ano=0}/{mes=0}")]
    public async Task<IActionResult> RelatorioEvolucao(int ano=0, int mes=0)
    {
        return PartialView();
    }

    [HttpGet("/relatorios/mapafinanceiro/{ano=0}/{mes=0}")]
    public async Task<IActionResult> MapaFinanceiro(int ano = 0, int mes = 0)
    {
        return PartialView();
    }

}
