using LGMDomains.Common;
using LGMDomains.Common.Helpers;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;
using LGMPulse.Domain.ViewModels;
using LGMPulse.Persistence.Interfaces;
using LGMPulse.Persistence.Repositories;
using System.Globalization;

namespace LGMPulse.AppServices.Services;

internal class MovtoService : BaseService<Movto>, IMovtoService
{
    private readonly IMovtoRepository _movtoRepository;
    private readonly IGrupoRepository _grupoRepository;
    public MovtoService(IMovtoRepository movtoRepository, IGrupoRepository grupoRepository) 
        : base(movtoRepository)
    {
        _movtoRepository = movtoRepository;
        _grupoRepository = grupoRepository;
    }

    public override async Task<ILGMResult> CreateAsync(Movto movto)
    {
        using (var transCtx = TransactionContext.NewTransaction())
        {
            await _movtoRepository.CreateTransactionalAsync(transCtx, movto);
            var grupo = await _grupoRepository.GetByIDContextualAsync(transCtx, movto.IDGrupo);
            if (grupo != null)
            {
                grupo.QtdMovtos = (grupo.QtdMovtos ?? 0) + 1;
                grupo.DateUltMovto = DateTimeHelper.Now();
                await _grupoRepository.UpdateTransactionalAsync(transCtx, grupo);
            }
            if (!await transCtx.ExecuteTransactionAsync())
                throw new Exception("Falha geral na execução da tarefa");
        }
        return LGMResult.Ok();
    }

    public async Task<LGMResult<List<Movto>>> GetListAsync(int year, int month, TipoMovtoEnum? tipoMovto = null)
    {
        var dataIni = new DateTime(year, month, 1);
        var dataFim = dataIni.AddMonths(1).AddSeconds(-1);
        var lista = await _movtoRepository.GetListAsync(
                            new Movto { DataMovto = dataIni, TipoMovto = tipoMovto },
                            new Movto { DataMovto = dataFim, TipoMovto = tipoMovto }
                         );
        return LGMResult.Ok(lista.OrderByDescending(x => x.DataMovto).ToList());
    }

    public async Task<LGMResult<RelatGrupoViewModel>> GetRelatGrupoViewModelAsync(int year, int month)
    {
        if (year == 0) { year = DateTimeHelper.Now().Year; }
        if (month == 0) { month = DateTimeHelper.Now().Month; }
        var dataIni = new DateTime(year, month, 1);
        var dataFim = dataIni.AddMonths(1).AddSeconds(-1);
        var sumarioGrupos = await _movtoRepository.GetListGrupoSumary(dataIni, dataFim);
        RelatGrupoViewModel relatViewModel = new()
        {
            Year = year,
            Month = month,
            MesReferencia = DateTimeHelper.MesReferencia(year, month),
            Grupos = sumarioGrupos
        };
        return LGMResult.Ok(relatViewModel);
    }

    public async Task<LGMResult<SumarioMes>> GetSumarioMesAsync(int year, int month)
    {
        var dataIni = new DateTime(year, month, 1);
        var dataFim = dataIni.AddMonths(1).AddSeconds(-1);
        var result = await _movtoRepository.GetSumarioMes(dataIni, dataFim);
        return LGMResult.Ok(result);
    }

    public async Task<LGMResult<RelatEvolucaoViewModel>> GetSumarioPeriodoAsync(DateTime dataIni, DateTime dataFim)
    {
        List<SumarioPeriodo> sumario = await _movtoRepository.GetSumarioPeriodo(dataIni, dataFim); 
        
        RelatEvolucaoViewModel viewModel = new();
        foreach (var item in sumario)
        {
            string mesRef = DateTimeHelper.MesReferencia(item.Ano, item.Mes).Substring(0, 3);
            viewModel.Receitas.Add(new EvolucaoSumary() { Year = item.Ano, Month = item.Mes, MesReferencia = mesRef, ValorTotal = item.TotalReceitas });
            viewModel.Despesas.Add(new EvolucaoSumary() { Year = item.Ano, Month = item.Mes, MesReferencia = mesRef, ValorTotal = item.TotalDespesas });
            viewModel.Liquidez.Add(new EvolucaoSumary() { Year = item.Ano, Month = item.Mes, MesReferencia = mesRef, ValorTotal = (item.TotalReceitas - item.TotalDespesas) });
        }

        var maxRec = viewModel.Receitas.Any() ? viewModel.Receitas.Max(x => x.ValorTotal) * 1.2m : 1m;
        var maxDesp = viewModel.Receitas.Any() ? viewModel.Despesas.Max(x => x.ValorTotal) * 1.2m : 1m;
        viewModel.ValMaxRecDesp = Math.Max(maxRec, maxDesp);

        return LGMResult.Ok(viewModel);
    }

    public async Task<LGMResult<MapaFinanceiroViewModel>> GetMapaFinanceiroAsync(int year)
    {
        var today = DateTimeHelper.Now();
        MapaFinanceiroViewModel viewModel = new()
        {
            Year = year,
            LastMonth = (year == today.Year ? today.Month : year < today.Year ? 12 : 0),
            Despesas = new(),
            Receitas = new(),
            Mapas = new(),
        };

        if (year > today.Year) return LGMResult.Ok(viewModel);

        viewModel.Mapas = await _movtoRepository.GetMapaFinanceiroAsync(year);

        if (viewModel.Mapas != null)
        {
            int lastMonth = viewModel.LastMonth;
            foreach (var mapa in viewModel.Mapas)
            {
                for (int i = 0; i < 12; i++)
                {
                    var totMes = mapa.TotalMes[i];
                    mapa.TotalMes[12] += totMes;
                    mapa.TotalMes[13] = (lastMonth > 0 ? mapa.TotalMes[12] / lastMonth : 0);

                    if (mapa.TipoMovto == TipoMovtoEnum.Despesa)
                    {
                        viewModel.Despesas.TotalMes[i] += totMes;
                        viewModel.Despesas.TotalMes[12] += totMes;
                    }
                    else
                    {
                        viewModel.Receitas.TotalMes[i] += totMes;
                        viewModel.Receitas.TotalMes[12] += totMes;
                    }
                }
                if (lastMonth > 0) // calcular média do subgrupo 
                {
                    viewModel.Receitas.TotalMes[13] = viewModel.Receitas.TotalMes[12] / lastMonth;
                    viewModel.Despesas.TotalMes[13] = viewModel.Despesas.TotalMes[12] / lastMonth;
                }
            }
        }

        return LGMResult.Ok(viewModel);
    }

}
