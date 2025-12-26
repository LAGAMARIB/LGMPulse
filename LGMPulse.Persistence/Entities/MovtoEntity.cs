using LGMDAL;

namespace LGMPulse.Persistence.Entities;

[LGMTableName("movto")]
internal class MovtoEntity : BaseEntity
{
    [LGMSearchField]
    public DateTime? DataMovto { get; set; }

    [LGMSearchField]
    public int? TipoMovto { get; set; }

    [LGMSearchField]
    public int? IDGrupo { get; set; }

    [LGMSearchField]
    public int? IDAgenda { get; set; }

    public string? Descricao { get; set; }

    public decimal? ValorMovto { get; set; }

    #region JoinTypes
    public JoinType<string> NomeGrupo { get; set; }
    public JoinType<string> ImagePathGrupo { get; set; }
    #endregion

    public MovtoEntity()
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
