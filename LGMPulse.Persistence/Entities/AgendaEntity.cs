using LGMDAL;

namespace LGMPulse.Persistence.Entities;

[LGMTableName("agenda")]
internal class AgendaEntity : BaseEntity
{
    public DateTime? DataMovto { get; set; }

    [LGMSearchField]
    public DateTime? DataVencto { get; set; }

    [LGMSearchField]
    public int? TipoMovto { get; set; }

    [LGMSearchField]
    public int? IDGrupo { get; set; }

    public string? Descricao { get; set; }

    [LGMSearchField]
    public bool? Recorrente { get; set; }

    [LGMSearchField]
    public string? IDRecorrencia { get; set; }

    [LGMSearchField]
    public int? Parcela { get; set; }

    public int? QtdParcelas { get; set; }

    [LGMSearchField]
    public int? StatusParcela { get; set; }

    public decimal? ValorParcela { get; set; }

    #region JoinTypes
    public JoinType<string> NomeGrupo { get; set; }
    public JoinType<string> ImagePathGrupo { get; set; }
    #endregion

    public AgendaEntity()
    {
        NomeGrupo = new()
        {
            ExtTable = "grupo",
            ExtField = nameof(GrupoEntity.Descricao),
            LocalKey = nameof(this.IDGrupo),
            Mandatory = false
        };

        ImagePathGrupo = new()
        {
            ExtTable = "grupo",
            ExtField = nameof(GrupoEntity.ImagePath),
            LocalKey = nameof(this.IDGrupo),
            Mandatory = false
        };
    }
}
