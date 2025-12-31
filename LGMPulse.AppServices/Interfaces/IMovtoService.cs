using LGMDomains.Common;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;
using LGMPulse.Domain.ViewModels;

namespace LGMPulse.AppServices.Interfaces;

public interface IMovtoService : IBaseService<Movto>
{
    Task<LGMResult<List<Movto>>> GetListAsync(int year, int month, TipoMovtoEnum? tipoMovto=null);
    Task<LGMResult<SumarioMes>> GetSumarioMesAsync(int year, int month);
    Task<LGMResult<RelatGrupoViewModel>> GetRelatGrupoViewModelAsync(int year, int month);
    Task<LGMResult<RelatEvolucaoViewModel>> GetSumarioPeriodoAsync(DateTime dataIni, DateTime dataFim);
}
