using LGMDAL;
using LGMDAL.MySQL;
using LGMPulse.Connections;
using LGMPulse.Domain.Domains;
using LGMPulse.Persistence.Entities;
using LGMPulse.Persistence.Interfaces;

namespace LGMPulse.Persistence.Repositories;

internal class LocalUserRepository : ILocalUserRepository
{
    private DBContext NewDBContext()
    {
        var _strConnName = ConnectionSettings.Instance.ConnectionName;
        return new DBContext(_strConnName, "", "");
    }

    public async Task<LocalUser?> GetByEmailAsync(string email)
    {
        var filter = new LocalUserEntity { UserEmail = email };
        using (var ctx = NewDBContext())
        {
            var entity = await ctx.GetFirstAsync(filter);
            return entity?.MapTo<LocalUser>();
        }
    }
}