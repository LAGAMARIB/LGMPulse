using LGMDomains.Common;
using LGMPulse.Domain.Enuns;

namespace LGMPulse.Domain.Domains;

public class Grupo: BaseDomain
{
    public TipoMovtoEnum? TipoMovto { get; set; }
    public string? Descricao { get; set; }
    public DateTime? DateUltMovto { get; set; }
    public int? QtdMovtos { get; set; }
    public string? ImagePath { get; set; }

    public double ScoreOrder { get; set; }
}
