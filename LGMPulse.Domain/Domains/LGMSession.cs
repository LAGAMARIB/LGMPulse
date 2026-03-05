using LGMDomains.Identity;

namespace LGMPulse.Domain.Domains;

public class LGMSession
{
    public LGMUser User { get; set; }
    public DateTime ExpireDateTime { get; set; }
}