using LGMPulse.Domain.Domains;

namespace LGMPulse.Persistence.Interfaces;

public interface ILocalUserRepository
{
    Task<LocalUser?> GetByEmailAsync(string email);
}