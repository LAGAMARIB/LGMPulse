using LGMDomains.Common;

namespace LGMPulse.Domain.Domains;

public class Grupo: BaseDomain
{
    public int? TipoMovto { get; set; }
    public string? Descricao { get; set; }
    public DateTime? DateUltMovto { get; set; }
    public int? QtdMovtos { get; set; }
}
