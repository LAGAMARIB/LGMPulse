using LGMDomains.Common;

namespace LGMPulse.Domain.Domains;

public class LocalUser : BaseDomain
{
    public string? UserLogin { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? DBKey { get; set; }
    public string? Token { get; set; }
    public int? SubscriptLevel { get; set; }
}
