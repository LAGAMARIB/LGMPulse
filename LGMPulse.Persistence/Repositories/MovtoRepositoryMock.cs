using LGMPulse.Domain.Domains;
using LGMPulse.Domain.ViewModels;
using LGMPulse.Persistence.Interfaces;

namespace LGMPulse.Persistence.Repositories;

internal class MovtoRepositoryMock : IMovtoRepository
{
    private static List<Movto> _listaMock = new List<Movto>();

    
    public Task<int?> CreateAsync(Movto domain, List<string>? changedFields = null)
    {
        int newID = _listaMock.Any()
            ? _listaMock.Max(g => g.ID.Value) + 1
            : 1;
        domain.ID = newID;
        _listaMock.Add(domain);
        return Task.FromResult<int?>(newID);
    }

    public Task<int?> CreateContextualAsync(TransactionContext trctx, Movto domain)
    {
        int newID = _listaMock.Any()
            ? _listaMock.Max(g => g.ID.Value) + 1
            : 1;
        domain.ID = newID;
        _listaMock.Add(domain);
        return Task.FromResult<int?>(newID);
    }

    public Task<int?> CreateTransactionalAsync(TransactionContext trctx, Movto domain, List<string>? changedFields = null)
    {
        int newID = _listaMock.Any()
            ? _listaMock.Max(g => g.ID.Value) + 1
            : 1;
        domain.ID = newID;
        _listaMock.Add(domain);
        return Task.FromResult<int?>(newID);
    }

    public Task DeleteAsync(Movto domain)
    {
        throw new NotImplementedException();
    }

    public Task DeleteContextualAsync(TransactionContext trctx, Movto domain)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTransactionalAsync(TransactionContext trctx, Movto domain)
    {
        throw new NotImplementedException();
    }

    public Task<Movto?> GetByIDAsync(int? id)
    {
        throw new NotImplementedException();
    }

    public Task<Movto?> GetByIDContextualAsync(TransactionContext trctx, int? id)
    {
        throw new NotImplementedException();
    }

    public Task<Movto?> GetFirstAsync(Movto objSelecIni, string? pSort = null, List<string>? fields = null)
    {
        throw new NotImplementedException();
    }

    public Task<Movto?> GetFirstContextualAsync(TransactionContext trctx, Movto objSelecIni, string? pSort = null, List<string>? fields = null)
    {
        throw new NotImplementedException();
    }

    public Task<Movto?> GetFirstInRangeAsync(Movto objSelecIni, Movto? objSelecFim, string? pSort = null, List<string>? fields = null)
    {
        throw new NotImplementedException();
    }

    public Task<Movto?> GetFirstInRangeContextualAsync(TransactionContext trctx, Movto objSelecIni, Movto? objSelecFim, string? pSort = null, List<string>? fields = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<Movto>> GetListAsync(Movto? objSelecIni = null, Movto? objSelecFim = null, string? pSort = null, List<string>? fields = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<Movto>> GetListContextualAsync(TransactionContext trctx, Movto? objSelecIni = null, Movto? objSelecFim = null, string? pSort = null, List<string>? fields = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<SumarioMes>> GetSumario(DateTime dataIni, DateTime dataFim)
    {
        throw new NotImplementedException();
    }

    public Task<int?> UpdateAsync(Movto domain, List<string>? changedFields = null)
    {
        throw new NotImplementedException();
    }

    public Task<int?> UpdateContextualAsync(TransactionContext trctx, Movto domain, List<string>? changedFields = null)
    {
        throw new NotImplementedException();
    }

    public Task<int?> UpdateTransactionalAsync(TransactionContext trctx, Movto domain, List<string>? changedFields = null)
    {
        throw new NotImplementedException();
    }
}
