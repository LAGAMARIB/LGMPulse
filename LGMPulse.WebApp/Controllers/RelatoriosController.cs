using LGMDomains.Common;
using LGMDomains.Common.Exceptions;
using LGMDomains.Common.Helpers;
using LGMPulse.AppServices.Helpers;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Enuns;
using LGMPulse.Domain.ViewModels;
using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IO.Pipes;
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
        vm.IsFreeMode = LocalUserHelper.GetLocalUser().SubscriptLevel == 0;

        return Task.FromResult( LGMResult.Ok(vm) );
    }

    [HttpGet("relatorios/extrato/{ano}/{mes}/{tipoMovto=null}")]
    public async Task<IActionResult> ExtratoAsync(int ano, int mes, TipoMovtoEnum? tipoMovto=null)
    {
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync(() => GetExtratoViewModelAsync(ano, mes, tipoMovto ) )
        );
    }

    private async Task<LGMResult<ExtratoViewModel>> GetExtratoViewModelAsync(int ano, int mes, TipoMovtoEnum? tipoMovto)
    {
        var result = await _movtoService.GetListAsync(ano, mes, tipoMovto );
        ExtratoViewModel viewModel = new()
        {
            Year = ano,
            Month = mes,
            Movtos = result.Data ?? new(),
            MovtoReferencia = tipoMovto == null ? "EXTRATO" : tipoMovto == TipoMovtoEnum.Receita ? "RECEITAS" : "DESPESAS"
        };
        return LGMResult.Ok( viewModel );
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
        bool isFreeMode = LocalUserHelper.GetLocalUser().SubscriptLevel == 0;
        DateTime dataBase = new DateTime(ano, mes, 01);
        DateTime dataIni = isFreeMode ? dataBase.AddMonths(-2) : dataBase.AddMonths(-6);
        DateTime dataFim = dataBase.AddMonths(1).AddSeconds(-1);

        var result = await _movtoService.GetSumarioPeriodoAsync(dataIni, dataFim);
        RelatEvolucaoViewModel viewModel = result.Data ?? new();
        viewModel.IsFreeMode = isFreeMode;
        return PartialView(viewModel);
    }

    [HttpGet("/relatorios/mapafinanceiro/{ano=0}/{mes=0}")]
    public async Task<IActionResult> MapaFinanceiro(int ano = 0, int mes = 0)
    {
        DateTime today = DateTimeHelper.Now();
        if (ano > today.Year)
            throw new RuleException("Período inválido");

        LGMResult<MapaFinanceiroViewModel> result = await _movtoService.GetMapaFinanceiroAsync(ano);
        MapaFinanceiroViewModel viewModel = result.Data ?? new();

        int firstMonthToShow = 1;
        int lastMonthToShow = 12;
        bool isFreeMode = LocalUserHelper.GetLocalUser().SubscriptLevel == 0;

        if (isFreeMode)
        {
            if (ano == today.Year)
            {
                int todayMonth = today.Month;
                if (todayMonth == 1)
                {
                    firstMonthToShow = 1;
                    lastMonthToShow = 3;
                }
                else if (todayMonth == 12)
                {
                    firstMonthToShow = 10;
                    lastMonthToShow = 12;
                }
                else
                {
                    firstMonthToShow = Math.Max(todayMonth - 1, 1);
                    lastMonthToShow = Math.Min(todayMonth + 1, 12);
                }
            }
            else // anos anteriores
            {
                firstMonthToShow = 10;
                lastMonthToShow = 12;
            }
        }

        viewModel.FirstMonthToShow = firstMonthToShow;
        viewModel.LastMonthToShow = lastMonthToShow;
        viewModel.IsFreeMode = isFreeMode;

        return PartialView(viewModel);
    }

}
