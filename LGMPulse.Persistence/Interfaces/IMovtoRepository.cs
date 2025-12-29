using LGMPulse.Domain.Domains;
using LGMPulse.Domain.ViewModels;

namespace LGMPulse.Persistence.Interfaces;

public interface IMovtoRepository : IBaseRepository<Movto>
{
    Task<List<SumarioMes>> GetSumario(DateTime dataIni, DateTime dataFim);
}
