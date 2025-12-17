using LGMPulse.Domain.Domains;
using LGMPulse.Persistence.Entities;
using LGMPulse.Persistence.Interfaces;

namespace LGMPulse.Persistence.Repositories;

internal class MovtoRepository : BaseRepository<Movto, MovtoEntity>, IMovtoRepository
{
}
