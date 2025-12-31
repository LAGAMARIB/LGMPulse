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
        var culture = new CultureInfo("pt-BR");

        var vm = new RelatoriosViewModel
        {
            Year = y,
            Month = m,
            IsMesAtual = (y == hoje.Year && m == hoje.Month),
            MesReferencia = culture.DateTimeFormat.GetMonthName(m).ToUpperInvariant() + " / " + y.ToString(),
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
        RelatEvolucaoViewModel viewModel = new();
        viewModel.Receitas = [
            new EvolucaoSumary() { Year = 2025, Month = 12, MesReferencia = "DEZ", ValorTotal= 150m },
            new EvolucaoSumary() { Year = 2025, Month = 11, MesReferencia = "NOV", ValorTotal= 170m },
            new EvolucaoSumary() { Year = 2025, Month = 10, MesReferencia = "OUT", ValorTotal= 140m },
            new EvolucaoSumary() { Year = 2025, Month = 09, MesReferencia = "SET", ValorTotal= 156m },
            new EvolucaoSumary() { Year = 2025, Month = 08, MesReferencia = "AGO", ValorTotal= 159m },
            new EvolucaoSumary() { Year = 2025, Month = 08, MesReferencia = "JUL", ValorTotal= 190m },
        ];

        viewModel.Despesas = [
            new EvolucaoSumary() { Year = 2025, Month = 12, MesReferencia = "DEZ", ValorTotal= 1150m },
            new EvolucaoSumary() { Year = 2025, Month = 11, MesReferencia = "NOV", ValorTotal= 1170m },
            new EvolucaoSumary() { Year = 2025, Month = 10, MesReferencia = "OUT", ValorTotal= 1140m },
            new EvolucaoSumary() { Year = 2025, Month = 09, MesReferencia = "SET", ValorTotal= 1156m },
            new EvolucaoSumary() { Year = 2025, Month = 08, MesReferencia = "AGO", ValorTotal= 1159m },
            new EvolucaoSumary() { Year = 2025, Month = 08, MesReferencia = "JUL", ValorTotal= 1190m },
        ];

        viewModel.Liquidez = [
            new EvolucaoSumary() { Year = 2025, Month = 12, MesReferencia = "DEZ", ValorTotal= 50m },
            new EvolucaoSumary() { Year = 2025, Month = 11, MesReferencia = "NOV", ValorTotal= 30m },
            new EvolucaoSumary() { Year = 2025, Month = 10, MesReferencia = "OUT", ValorTotal= 20m },
            new EvolucaoSumary() { Year = 2025, Month = 09, MesReferencia = "SET", ValorTotal= -15m },
            new EvolucaoSumary() { Year = 2025, Month = 08, MesReferencia = "AGO", ValorTotal= -5m },
            new EvolucaoSumary() { Year = 2025, Month = 08, MesReferencia = "JUL", ValorTotal= 40m },
        ];

        var maxRec = viewModel.Receitas.Max(x => x.ValorTotal) * 1.2m;
        var maxDesp = viewModel.Despesas.Max(x => x.ValorTotal) * 1.2m;
        viewModel.ValMaxRecDesp = Math.Max(maxRec, maxDesp);

        return PartialView(viewModel);
    }

    [HttpGet("/relatorios/mapafinanceiro/{ano=0}/{mes=0}")]
    public async Task<IActionResult> MapaFinanceiro(int ano = 0, int mes = 0)
    {
        return PartialView();
    }

}
