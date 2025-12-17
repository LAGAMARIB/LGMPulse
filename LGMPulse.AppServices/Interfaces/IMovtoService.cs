using LGMDomains.Common;
using LGMPulse.Domain.Domains;

namespace LGMPulse.AppServices.Interfaces;

public interface IMovtoService : IBaseService<Movto>
{
    Task<LGMResult<List<Movto>>> GetListAsync(int year, int month);
}
