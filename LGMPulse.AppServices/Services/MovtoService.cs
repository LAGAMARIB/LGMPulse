using LGMDomains.Common;
using LGMDomains.Common.Helpers;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.Persistence.Interfaces;
using LGMPulse.Persistence.Repositories;

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

    public override async Task<ILGMResult> CreateAsync(Movto domain)
    {
        using (var transCtx = TransactionContext.NewTransaction())
        {
            await _movtoRepository.CreateTransactionalAsync(transCtx, domain);
            var grupo = await _grupoRepository.GetByIDContextualAsync(transCtx, domain.ID);
            if (grupo != null)
            {
                grupo.QtdMovtos += 1;
                grupo.DateUltMovto = DateTimeHelper.Now();
                await _grupoRepository.UpdateTransactionalAsync(transCtx, grupo);
            }
            if (!await transCtx.ExecuteTransactionAsync())
                throw new Exception("Falha geral na execução da tarefa");
        }
        return LGMResult.Ok();
    }

    public async Task<LGMResult<List<Movto>>> GetListAsync(int year, int month)
    {
        var dateIni = new DateTime(year, month, 1);
        var dataFim = dateIni.AddMonths(1).AddSeconds(-1);
        var lista = await _movtoRepository.GetListAsync(
                            new Movto { DataMovto = dateIni },
                            new Movto { DataMovto = dataFim }
                         );
        return LGMResult.Ok(lista);
    }
}
