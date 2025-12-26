using LGMDomains.Common;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;

namespace LGMPulse.AppServices.Interfaces;

public interface IMovtoService : IBaseService<Movto>
{
    Task<LGMResult<List<Movto>>> GetListAsync(int year, int month, TipoMovtoEnum? tipoMovto=null);
}
