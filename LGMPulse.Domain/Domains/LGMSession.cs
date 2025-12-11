namespace LGMPulse.Domain.Domains;

public class LGMSession
{
    public LocalUser User { get; set; }
    public DateTime ExpireDateTime { get; set; }
}