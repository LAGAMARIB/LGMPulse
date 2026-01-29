using LGMDomains.Common;
using LGMPulse.Domain.Domains;

namespace LGMPulse.AppServices.Interfaces;

public interface IGrupoService : IBaseService<Grupo>
{
    Task<LGMResult<List<Grupo>>> GetListOrderedAsync(Grupo? filterIni = null, Grupo? filterFim = null, string? sortBy = null, List<string>? fields = null);
}
