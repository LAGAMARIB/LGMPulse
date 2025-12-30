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
        var culture = new CultureInfo("pt-BR");
        RelatGrupoViewModel relatViewModel = new()
        {
            Year = year,
            Month = month,
            MesReferencia = culture.DateTimeFormat.GetMonthName(month).ToUpperInvariant() + " / " + year.ToString(),
            Grupos = sumarioGrupos
        };
        return LGMResult.Ok(relatViewModel);
    }

    public async Task<LGMResult<SumarioMes>> GetSumarioMesAsync(int year, int month)
    {
        var dataIni = new DateTime(year, month, 1);
        var dataFim = dataIni.AddMonths(1).AddSeconds(-1);
        var result = await _movtoRepository.GetSumario(dataIni, dataFim);
        return LGMResult.Ok(result);
    }
}
