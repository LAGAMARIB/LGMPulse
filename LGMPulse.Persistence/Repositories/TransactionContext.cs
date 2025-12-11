using LGMDAL.Interfaces;
using LGMDAL.MySQL;
using LGMPulse.Connections;
using LGMPulse.Connections.Helpers;

namespace LGMPulse.Persistence.Repositories;

public class TransactionContext : IDisposable
{
    internal IDBContext DBContext { get; }

    private TransactionContext(IDBContext ctx)
    {
        DBContext = ctx;
    }

    public static TransactionContext NewTransaction()
    {
        var _user = SessionHelperAccessor.Current.GetLGMSession()?.User
            ?? throw new UnauthorizedAccessException("TransactionContext: Usuário não autenticado.");
        var ctx = new DBContext(ConnectionSettings.Instance.ConnectionName, _user?.DBKey, _user?.UserLogin);
        return new TransactionContext(ctx);
    }

    public async Task<bool> ExecuteTransactionAsync()
    {
        return await DBContext.ExecuteTransactionAsync();
    }

    public void Dispose()
    {
        DBContext.Dispose();
    }

}