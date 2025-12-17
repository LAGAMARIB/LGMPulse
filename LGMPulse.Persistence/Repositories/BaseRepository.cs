using LGMDAL;
using LGMDAL.Interfaces;
using LGMDAL.MySQL;
using LGMDomains.Common;
using LGMPulse.Connections;
using LGMPulse.Connections.Helpers;
using LGMPulse.Persistence.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LGMPulse.Persistence.Repositories;

internal abstract class BaseRepository<TDomain, TEntity> : IBaseRepository<TDomain>
    where TEntity : BaseEntity, new()
    where TDomain : BaseDomain, new()
{
    #region Fields
    private readonly string? _strConnection;
    //protected readonly LocalUser? _user;
    private IDBContext? _ctx;

    private SessionHelper? _sessionHelper;
    protected SessionHelper SessionHelper =>
            _sessionHelper ??=
                new HttpContextAccessor().HttpContext!.RequestServices.GetRequiredService<SessionHelper>();
    #endregion

    #region Initialize

    protected DBContext NewDBContext()
    {
        //var _user = SessionHelper.GetLGMSession()?.User ?? throw new UnauthorizedAccessException("BaseRepository: Usuário não autenticado.");
        var _user = SessionHelper.GetLGMSession_Cookie()?.User;
        if (_user == null)
            throw new UnauthorizedAccessException("BaseRepository: Usuário não autenticado.");

        return new DBContext(ConnectionSettings.Instance.ConnectionName, _user?.DBKey, _user?.UserLogin);
    }

    #endregion

    #region TaskAsync
    public virtual async Task<List<TDomain>> GetListAsync(
        TDomain? objSelecIni = default(TDomain),
        TDomain? objSelecFim = default(TDomain),
        string? pSort = null,
        List<string>? fields = null)
    {
        List<TDomain> result = new();
        TEntity? startEntity = objSelecIni is null ? null : LGMMapper.MapToNewEntity<TEntity>(objSelecIni);
        TEntity? endEntity = objSelecFim is null ? null : LGMMapper.MapToNewEntity<TEntity>(objSelecFim);

        using (var ctx = NewDBContext())
        {
            var entities = await ctx.GetListAsync<TEntity>(startEntity, endEntity, pSort, fields);
            foreach (var entity in entities)
            {
                result.Add(entity.MapTo<TDomain>());
            }
            return result;
        }
    }

    public virtual async Task<TDomain?> GetFirstInRangeAsync(TDomain objSelecIni,
                                                      TDomain? objSelecFim,
                                                      string? pSort = null,
                                                      List<string>? fields = null)
    {
        TDomain result = new();
        TEntity? startEntity = objSelecIni is null ? null : LGMMapper.MapToNewEntity<TEntity>(objSelecIni);
        TEntity? endEntity = objSelecFim is null ? null : LGMMapper.MapToNewEntity<TEntity>(objSelecFim);

        using (var ctx = NewDBContext())
        {
            var listEntities = await ctx.GetListAsync<TEntity>(startEntity, endEntity, pSort, fields);
            var entity = listEntities.FirstOrDefault();
            return entity?.MapTo<TDomain>();
        }
    }

    public virtual async Task<TDomain?> GetFirstAsync(TDomain objSelec,
                                                      string? pSort = null,
                                                      List<string>? fields = null)
    {
        TEntity startEntity = LGMMapper.MapToNewEntity<TEntity>(objSelec);
        using (var ctx = NewDBContext())
        {
            var entity = await ctx.GetFirstAsync(startEntity, pSort, fields);
            return entity?.MapTo<TDomain>();
        }
    }

    public virtual async Task<TDomain?> GetByIDAsync(int? id)
    {
        using (var ctx = NewDBContext())
        {
            var entity = await ctx.GetByIDAsync<TEntity>(id);
            return entity?.MapTo<TDomain>();
        }
    }

    public virtual async Task<int?> CreateAsync(TDomain domain, List<string>? changedFields = null)
    {
        if (domain == null) throw new ArgumentNullException($"Objeto {typeof(TDomain).Name} vazio");

        var entity = LGMMapper.MapToNewEntity<TEntity>(domain);
        using (var ctx = NewDBContext())
        {
            var newId = await ctx.CreateAsync(entity, changedFields);
            return newId;
        }
    }

    public virtual async Task<int?> UpdateAsync(TDomain domain, List<string>? changedFields = null)
    {
        if (domain == null) throw new ArgumentNullException($"Objeto {typeof(TDomain).Name} vazio");

        using (var ctx = NewDBContext())
        {
            var entity = await ctx.GetByIDAsync<TEntity>(domain.ID);
            if (entity == null)
                throw new Exception($"Registro de {typeof(TEntity).Name} não disponível");

            LGMMapper.MapToUpdateEntity(domain, entity);
            await ctx.UpdateAsync(entity, changedFields);
            return domain.ID;
        }
    }

    public virtual async Task DeleteAsync(TDomain domain)
    {
        using (var ctx = NewDBContext())
        {
            var entity = await ctx.GetByIDAsync<TEntity>(domain.ID);
            if (entity == null)
                throw new Exception($"Registro de {typeof(TEntity).Name} não disponível");

            await ctx.DeleteAsync(entity);
            ctx.Dispose();
        }
    }
    #endregion

    #region Contextual
    public virtual async Task<List<TDomain>> GetListContextualAsync(TransactionContext trctx,
                                                                    TDomain? objSelecIni = default(TDomain),
                                                                    TDomain? objSelecFim = default(TDomain),
                                                                    string? pSort = null,
                                                                    List<string>? fields = null)
    {
        List<TDomain> result = new();
        TEntity? startEntity = objSelecIni is null ? null : LGMMapper.MapToNewEntity<TEntity>(objSelecIni);
        TEntity? endEntity = objSelecFim is null ? null : LGMMapper.MapToNewEntity<TEntity>(objSelecFim);

        var ctx = trctx.DBContext;
        var entities = await ctx.GetListAsync<TEntity>(startEntity, endEntity, pSort, fields);
        foreach (var entity in entities)
        {
            result.Add(entity.MapTo<TDomain>());
        }

        return result;
    }

    public virtual async Task<TDomain?> GetFirstInRangeContextualAsync(TransactionContext trctx,
                                                                       TDomain objSelecIni,
                                                                       TDomain? objSelecFim,
                                                                       string? pSort = null,
                                                                       List<string>? fields = null)
    {
        TDomain result = new();
        TEntity? startEntity = objSelecIni is null ? null : LGMMapper.MapToNewEntity<TEntity>(objSelecIni);
        TEntity? endEntity = objSelecFim is null ? null : LGMMapper.MapToNewEntity<TEntity>(objSelecFim);
        var ctx = trctx.DBContext;
        var listEntities = await ctx.GetListAsync<TEntity>(startEntity, endEntity, pSort, fields);
        var entity = listEntities.FirstOrDefault();
        return entity?.MapTo<TDomain>();
    }


    public virtual async Task<TDomain?> GetFirstContextualAsync(TransactionContext trctx,
                                                                TDomain objSelec,
                                                                string? pSort = null,
                                                                List<string>? fields = null)
    {
        TEntity startEntity = LGMMapper.MapToNewEntity<TEntity>(objSelec);
        var ctx = trctx.DBContext;
        var entity = await ctx.GetFirstAsync(startEntity, pSort, fields);
        return entity?.MapTo<TDomain>();
    }

    public virtual async Task<TDomain?> GetByIDContextualAsync(TransactionContext trctx, int? id)
    {
        var ctx = trctx.DBContext;
        var entity = await ctx.GetByIDAsync<TEntity>(id);
        return entity?.MapTo<TDomain>();
    }

    public async Task<int?> CreateContextualAsync(TransactionContext trctx, TDomain domain)
    {
        var ctx = trctx.DBContext;
        var entity = LGMMapper.MapToNewEntity<TEntity>(domain);
        var newId = await ctx.CreateAsync(entity);
        return newId;
    }

    public async Task DeleteContextualAsync(TransactionContext trctx, TDomain domain)
    {
        var ctx = trctx.DBContext;

        var entity = await ctx.GetByIDAsync<TEntity>(domain.ID);
        if (entity == null)
            throw new Exception($"Registro de {typeof(TEntity).Name} não disponível");

        await ctx.DeleteAsync(entity);
    }

    public virtual async Task<int?> UpdateContextualAsync(TransactionContext trctx, TDomain domain, List<string>? changedFields = null)
    {
        if (domain == null) throw new ArgumentNullException($"Objeto {typeof(TDomain).Name} vazio");
        var ctx = trctx.DBContext;

        var entity = await ctx.GetByIDAsync<TEntity>(domain.ID);
        if (entity == null)
            throw new Exception($"Registro de {typeof(TEntity).Name} não disponível");

        LGMMapper.MapToUpdateEntity(domain, entity);
        await ctx.UpdateAsync(entity, changedFields);
        return domain.ID;
    }

    #endregion

    #region Transactional
    public virtual async Task<int?> CreateTransactionalAsync(TransactionContext trctx, TDomain domain, List<string>? changedFields = null)
    {
        if (domain == null) throw new ArgumentNullException($"Objeto {typeof(TDomain).Name} vazio");
        var entity = LGMMapper.MapToNewEntity<TEntity>(domain);
        var ctx = trctx.DBContext;
        await ctx.NewTransaction_CreateAsync(entity, changedFields);
        return null;
    }

    public virtual async Task<int?> UpdateTransactionalAsync(TransactionContext trctx, TDomain domain, List<string>? changedFields = null)
    {
        if (domain == null) throw new ArgumentNullException($"Objeto {typeof(TDomain).Name} vazio");
        var ctx = trctx.DBContext;

        var entity = await ctx.GetByIDAsync<TEntity>(domain.ID);
        if (entity == null)
            throw new Exception($"Registro de {typeof(TEntity).Name} não disponível");

        LGMMapper.MapToUpdateEntity(domain, entity);
        await ctx.NewTransaction_UpdateAsync(entity, changedFields);
        return domain.ID;
    }

    public virtual async Task DeleteTransactionalAsync(TransactionContext trctx, TDomain domain)
    {
        var ctx = trctx.DBContext;

        var entity = await ctx.GetByIDAsync<TEntity>(domain.ID);
        if (entity == null)
            throw new Exception($"Registro de {typeof(TEntity).Name} não disponível");

        await ctx.NewTransaction_DeleteAsync(entity);
    }

    #endregion
}
