using LGMPulse.Domain.Domains;
using LGMPulse.Domain.ViewModels;

namespace LGMPulse.Persistence.Interfaces;

public interface IMovtoRepository : IBaseRepository<Movto>
{
    Task<SumarioMes?> GetSumario(DateTime dataIni, DateTime dataFim);
    Task<List<GrupoSumary>> GetListGrupoSumary(DateTime dataIni, DateTime dataFim);
}
