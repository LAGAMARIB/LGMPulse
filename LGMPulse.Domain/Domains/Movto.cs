using LGMDomains.Common;

namespace LGMPulse.Domain.Domains;

public class Movto : BaseDomain
{
    public DateTime? DataMovto { get; set; }
    public int? TipoMovto { get; set; }
    public int? IDGrupo { get; set; }
    public int? IDAgenda { get; set; }
    public string? Descricao { get; set; }     
    public decimal? ValorMovto { get; set; }   
}
