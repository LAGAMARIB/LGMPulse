using LGMDomains.Common;
using LGMPulse.Domain.Enuns;

namespace LGMPulse.Domain.Domains;

public class Movto : BaseDomain
{
    public DateTime? DataMovto { get; set; }
    public TipoMovtoEnum? TipoMovto { get; set; }
    public int? IDGrupo { get; set; }
    public int? IDAgenda { get; set; }
    public string? Descricao { get; set; }     
    public decimal? ValorMovto { get; set; }

    #region JoinTypes
    public string? NomeGrupo { get; set; }
    public string? ImagePathGrupo { get; set; }
    #endregion
}
