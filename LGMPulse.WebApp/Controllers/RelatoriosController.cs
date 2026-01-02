using LGMDomains.Common;
using LGMDomains.Common.Helpers;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Enuns;
using LGMPulse.Domain.ViewModels;
using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LGMPulse.WebApp.Controllers;

public class RelatoriosController : LGMController
{
    private readonly IMovtoService _movtoService;

    public RelatoriosController(IMovtoService movtoService)
    {
        _movtoService = movtoService;
    }

    [HttpGet("relatorios/{ano=null}/{mes=null}")]
    public async Task<IActionResult> Relatorios(int? ano=null, int? mes=null )
    {
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync(() => NewRelatoriosViewModel(ano, mes) )
        );
    }

    private Task<LGMResult<RelatoriosViewModel>> NewRelatoriosViewModel(int? ano, int? mes)
    {
        var hoje = DateTimeHelper.Now();

        int y = ano ?? hoje.Year;
        int m = mes ?? hoje.Month;

        var vm = new RelatoriosViewModel
        {
            Year = y,
            Month = m,
            IsMesAtual = (y == hoje.Year && m == hoje.Month),
            MesReferencia = DateTimeHelper.MesReferencia(y, m),
        };

        return Task.FromResult( LGMResult.Ok(vm) );
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
        var result = await _movtoService.GetRelatGrupoViewModelAsync(ano, mes);
        RelatGrupoViewModel viewModel = result.Data ?? new();
        return PartialView(viewModel);
    }

    [HttpGet("/relatorios/evolucao/{ano=0}/{mes=0}")]
    public async Task<IActionResult> RelatorioEvolucao(int ano=0, int mes=0)
    {
        DateTime dataBase = new DateTime(ano, mes, 01);
        DateTime dataIni = dataBase.AddMonths(-5);
        DateTime dataFim = dataBase.AddMonths(1).AddSeconds(-1);
        
        var result = await _movtoService.GetSumarioPeriodoAsync(dataIni, dataFim);
        RelatEvolucaoViewModel viewModel = result.Data ?? new();
        return PartialView(viewModel);
    }

    [HttpGet("/relatorios/mapafinanceiro/{ano=0}/{mes=0}")]
    public async Task<IActionResult> MapaFinanceiro(int ano = 0, int mes = 0)
    {
        return PartialView();
    }

}
