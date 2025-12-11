using LGMDomains.Common;
using LGMPulse.Persistence.Repositories;

namespace LGMPulse.Persistence.Interfaces;

public interface IBaseRepository<TDomain> where TDomain : BaseDomain, new()
{
    #region TaskAsync
    Task<List<TDomain>> GetListAsync(
        TDomain? objSelecIni = default(TDomain),
        TDomain? objSelecFim = default(TDomain),
        string? pSort = null,
        List<string>? fields = null);

    Task<TDomain?> GetByIDAsync(int? id);

    Task<int?> CreateAsync(TDomain domain, List<string>? changedFields = null);

    Task<int?> UpdateAsync(TDomain domain, List<string>? changedFields = null);

    Task DeleteAsync(TDomain domain);

    Task<TDomain?> GetFirstInRangeAsync(TDomain objSelecIni,
                                 TDomain? objSelecFim,
                                 string? pSort = null,
                                 List<string>? fields = null);

    Task<TDomain?> GetFirstAsync(TDomain objSelecIni,
                                 string? pSort = null,
                                 List<string>? fields = null);
    #endregion

    #region Contextual
    Task<List<TDomain>> GetListContextualAsync(
        TransactionContext trctx,
        TDomain? objSelecIni = default(TDomain),
        TDomain? objSelecFim = default(TDomain),
        string? pSort = null,
        List<string>? fields = null
    );

    Task<TDomain?> GetByIDContextualAsync(TransactionContext trctx, int? id);

    Task<TDomain?> GetFirstInRangeContextualAsync(TransactionContext trctx,
                                                  TDomain objSelecIni,
                                                  TDomain? objSelecFim,
                                                  string? pSort = null,
                                                  List<string>? fields = null);

    Task<TDomain?> GetFirstContextualAsync(TransactionContext trctx,
                                           TDomain objSelecIni,
                                           string? pSort = null,
                                           List<string>? fields = null);

    Task<int?> CreateContextualAsync(TransactionContext trctx, TDomain domain);

    Task DeleteContextualAsync(TransactionContext trctx, TDomain domain);

    Task<int?> UpdateContextualAsync(TransactionContext trctx, TDomain domain, List<string>? changedFields = null);
    #endregion

    #region Transactions
    //TransactionContext NewTransaction();

    Task<int?> CreateTransactionalAsync(TransactionContext trctx, TDomain domain, List<string>? changedFields = null);

    Task<int?> UpdateTransactionalAsync(TransactionContext trctx, TDomain domain, List<string>? changedFields = null);

    Task DeleteTransactionalAsync(TransactionContext trctx, TDomain domain);
    #endregion
}
