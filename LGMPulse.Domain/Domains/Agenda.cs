using LGMDomains.Common;
using LGMPulse.Domain.Enuns;

namespace LGMPulse.Domain.Domains;

public class Agenda : BaseDomain
{
    public DateTime? DataMovto { get; set; }

    public DateTime? DataVencto { get; set; }

    public TipoMovtoEnum? TipoMovto { get; set; }

    public int? IDGrupo { get; set; }

    public string? Descricao { get; set; }

    public bool? Recorrente { get; set; }

    public string? IDRecorrencia { get; set; }

    public int? Parcela { get; set; }

    public int? QtdParcelas { get; set; }

    public ParcelaStatusEnum? StatusParcela { get; set; }

    public decimal? ValorParcela { get; set; }

    #region JoinTypes
    public string? NomeGrupo { get; set; }
    public string? ImagePathGrupo { get; set; }
    #endregion

}
