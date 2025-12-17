using LGMDomains.Common.Exceptions;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;
using LGMPulse.Persistence.Interfaces;

namespace LGMPulse.Persistence.Repositories;

internal class GrupoRepositoryMock : IGrupoRepository
{
    private static List<Grupo> _listaMock = new List<Grupo>();

    public GrupoRepositoryMock()
    {
        _listaMock = new();
        _listaMock.Add(new Grupo { ID = 1,  TipoMovto = TipoMovtoEnum.Receita, Descricao = "Salário", ImagePath = "/icons/salary.svg" });
        _listaMock.Add(new Grupo { ID = 2,  TipoMovto = TipoMovtoEnum.Receita, Descricao = "Rendimentos", ImagePath = "/icons/rendimentos.svg" });
        _listaMock.Add(new Grupo { ID = 3,  TipoMovto = TipoMovtoEnum.Receita, Descricao = "Vendas", ImagePath = "/icons/seller.svg" });
        _listaMock.Add(new Grupo { ID = 4,  TipoMovto = TipoMovtoEnum.Receita, Descricao = "Outras receitas", ImagePath = "/icons/open-pocket.svg" });
        _listaMock.Add(new Grupo { ID = 5,  TipoMovto = TipoMovtoEnum.Despesa, Descricao = "Carro", ImagePath = "/icons/icon-car.svg" });
        _listaMock.Add(new Grupo { ID = 6,  TipoMovto = TipoMovtoEnum.Despesa, Descricao = "Moradia", ImagePath = "/icons/home.svg" });
        _listaMock.Add(new Grupo { ID = 7,  TipoMovto = TipoMovtoEnum.Despesa, Descricao = "Saúde", ImagePath = "/icons/aid.svg" });
        _listaMock.Add(new Grupo { ID = 8,  TipoMovto = TipoMovtoEnum.Despesa, Descricao = "Educação", ImagePath = "/icons/education.svg" });
        _listaMock.Add(new Grupo { ID = 9,  TipoMovto = TipoMovtoEnum.Despesa, Descricao = "Hobiew & Lazer", ImagePath = "/icons/game.svg" });
        _listaMock.Add(new Grupo { ID = 10, TipoMovto = TipoMovtoEnum.Despesa, Descricao = "Mercado", ImagePath = "/icons/shopping.svg" });
        _listaMock.Add(new Grupo { ID = 11, TipoMovto = TipoMovtoEnum.Despesa, Descricao = "Outras despesas", ImagePath = "/icons/misc-buy.svg" });
    }

    public Task<int?> CreateAsync(Grupo domain, List<string>? changedFields = null)
    {
        int newID = _listaMock.Any()
            ? _listaMock.Max(g => g.ID.Value) + 1
            : 1;
        domain.ID = newID;
        _listaMock.Add(domain);
        return Task.FromResult<int?>(newID);
    }

    public Task<int?> CreateContextualAsync(TransactionContext trctx, Grupo domain)
    {
        int newID = _listaMock.Any()
            ? _listaMock.Max(g => g.ID.Value) + 1
            : 1;
        domain.ID = newID;
        _listaMock.Add(domain);
        return Task.FromResult<int?>(newID);
    }

    public Task<int?> CreateTransactionalAsync(TransactionContext trctx, Grupo domain, List<string>? changedFields = null)
    {
        int newID = _listaMock.Any()
            ? _listaMock.Max(g => g.ID.Value) + 1
            : 1;
        domain.ID = newID;
        _listaMock.Add(domain);
        return Task.FromResult<int?>(newID);
    }

    public Task DeleteAsync(Grupo domain)
    {
        throw new NotImplementedException();
    }

    public Task DeleteContextualAsync(TransactionContext trctx, Grupo domain)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTransactionalAsync(TransactionContext trctx, Grupo domain)
    {
        throw new NotImplementedException();
    }

    public async Task<Grupo?> GetByIDAsync(int? id)
    {
        var grupo = _listaMock.FirstOrDefault(x => x.ID == id);
        return grupo;
    }

    public async Task<Grupo?> GetByIDContextualAsync(TransactionContext trctx, int? id)
    {
        var grupo = _listaMock.FirstOrDefault(x => x.ID == id);
        return grupo;
    }

    public Task<Grupo?> GetFirstAsync(Grupo objSelecIni, string? pSort = null, List<string>? fields = null)
    {
        throw new NotImplementedException();
    }

    public Task<Grupo?> GetFirstContextualAsync(TransactionContext trctx, Grupo objSelecIni, string? pSort = null, List<string>? fields = null)
    {
        throw new NotImplementedException();
    }

    public Task<Grupo?> GetFirstInRangeAsync(Grupo objSelecIni, Grupo? objSelecFim, string? pSort = null, List<string>? fields = null)
    {
        throw new NotImplementedException();
    }

    public Task<Grupo?> GetFirstInRangeContextualAsync(TransactionContext trctx, Grupo objSelecIni, Grupo? objSelecFim, string? pSort = null, List<string>? fields = null)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Grupo>> GetListAsync(Grupo? objSelecIni = null, Grupo? objSelecFim = null, string? pSort = null, List<string>? fields = null)
    {
        if (objSelecIni != null)
            return _listaMock.Where(x => x.TipoMovto == objSelecIni.TipoMovto).ToList();
        return _listaMock;
    }

    public async Task<List<Grupo>> GetListContextualAsync(TransactionContext trctx, Grupo? objSelecIni = null, Grupo? objSelecFim = null, string? pSort = null, List<string>? fields = null)
    {
        if (objSelecIni != null)
            return _listaMock.Where(x => x.TipoMovto == objSelecIni.TipoMovto).ToList();
        return _listaMock;
    }

    public async Task<int?> UpdateAsync(Grupo domain, List<string>? changedFields = null)
    {
        var grupo = _listaMock.FirstOrDefault(x => x.ID == domain.ID);
        if (grupo == null)
            throw new RuleException("Registro não encontrado");

        grupo = domain.Clone<Grupo>();
        return domain.ID;
    }

    public async Task<int?> UpdateContextualAsync(TransactionContext trctx, Grupo domain, List<string>? changedFields = null)
    {
        var grupo = _listaMock.FirstOrDefault(x => x.ID == domain.ID);
        if (grupo == null)
            throw new RuleException("Registro não encontrado");

        grupo = domain.Clone<Grupo>();
        return domain.ID;
    }

    public async Task<int?> UpdateTransactionalAsync(TransactionContext trctx, Grupo domain, List<string>? changedFields = null)
    {
        var grupo = _listaMock.FirstOrDefault(x => x.ID == domain.ID);
        if (grupo == null)
            throw new RuleException("Registro não encontrado");

        grupo = domain.Clone<Grupo>();
        return domain.ID;
    }
}
