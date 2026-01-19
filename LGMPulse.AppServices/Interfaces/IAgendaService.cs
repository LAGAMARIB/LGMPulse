using LGMDomains.Common;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.ViewModels;

namespace LGMPulse.AppServices.Interfaces;

public interface IAgendaService : IBaseService<Agenda>
{
    Task<ILGMResult> BaixarAsync(int id);
}
