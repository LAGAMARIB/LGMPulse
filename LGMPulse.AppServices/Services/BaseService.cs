using LGMDomains.Common;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Persistence.Interfaces;
using LGMPulse.Persistence.Repositories;

namespace LGMPulse.AppServices.Services;

internal class BaseService<TDomain> : IBaseService<TDomain> where TDomain : BaseDomain, new()
{
    private readonly IBaseRepository<TDomain> _repository;

    public BaseService(IBaseRepository<TDomain> baseRepository)
    {
        _repository = baseRepository;
    }

    public virtual async Task<LGMResult<TDomain>> GetByIdAsync(int id)
    {
        var data = await _repository.GetByIDAsync(id);
        if (data == null)
            return LGMResult.Fail<TDomain>();

        return LGMResult.Ok(data);
    }

    public virtual async Task<LGMResult<List<TDomain>>> GetListAsync(TDomain? filterIni = null, TDomain? filterFim = null, string? sortBy = null, List<string>? fields = null)
    {
        var data = await _repository.GetListAsync(filterIni, filterFim, sortBy, fields);
        return LGMResult.Ok(data);
    }

    public virtual async Task<ILGMResult> CreateAsync(TDomain domain)
    {
        var newId = await _repository.CreateAsync(domain);
        return LGMResult.Ok(newId);
    }

    public virtual async Task<ILGMResult> UpdateAsync(TDomain domain, List<string>? campos = null)
    {
        await _repository.UpdateAsync(domain, campos);
        return LGMResult.Ok();
    }

    public virtual async Task<ILGMResult> DeleteAsync(int? id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));
        using (var transCtx = TransactionContext.NewTransaction())
        {
            TDomain? domain = await _repository.GetByIDContextualAsync(transCtx, id);
            if (domain == null)
                throw new Exception("Registro não disponível para exclusão");
            await _repository.DeleteContextualAsync(transCtx, domain);
        }
        return LGMResult.Ok();
    }

    public async Task<LGMResult<TDomain>> GetFirstAsync(TDomain objSelecIni, TDomain objSelecFim, string? pSort = null, List<string>? fields = null)
    {
        var data = await _repository.GetFirstInRangeAsync(objSelecIni, objSelecFim, pSort, fields);
        if (data == null)
            return LGMResult.Fail<TDomain>();
        return LGMResult.Ok(data);
    }

    public async Task<LGMResult<TDomain>> GetFirstAsync(TDomain objSelecIni, string? pSort = null, List<string>? fields = null)
    {
        var data = await _repository.GetFirstAsync(objSelecIni, pSort, fields);
        if (data == null)
            return LGMResult.Fail<TDomain>();
        return LGMResult.Ok(data);
    }
}
