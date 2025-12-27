using LGMDomains.Common;
using LGMPulse.Domain;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;

namespace LGMPulse.AppServices.Interfaces;

public interface IMovtoService : IBaseService<Movto>
{
    Task<LGMResult<List<Movto>>> GetListAsync(int year, int month, TipoMovtoEnum? tipoMovto=null);
    Task<LGMResult<SumarioMes>> GetSumarioMesAsync(int year, int month);
}
