using LGMDAL;

namespace LGMPulse.Persistence.Entities;

[LGMTableName("grupo")]
internal class GrupoEntity : BaseEntity
{
    [LGMSearchField]
    public int? TipoMovto { get; set; }

    public string? Descricao { get; set; }

    [LGMSearchField]
    public DateTime? DateUltMovto { get; set; }

    [LGMSearchField]
    public int? QtdMovtos { get; set; }
}
