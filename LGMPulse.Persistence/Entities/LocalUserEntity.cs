using LGMDAL;

namespace LGMPulse.Persistence.Entities;

[LGMTableName("lgm_local_user")]
internal class LocalUserEntity : BaseEntity
{
    [LGMSearchField]
    public string? UserEmail { get; set; }

    public string? DBKey { get; set; }
}