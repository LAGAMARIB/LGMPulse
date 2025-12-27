using LGMPulse.Domain;
using LGMPulse.Domain.Domains;

namespace LGMPulse.Persistence.Interfaces;

public interface IMovtoRepository : IBaseRepository<Movto>
{
    Task<SumarioMes?> GetSumario(DateTime dataIni, DateTime dataFim);
}
