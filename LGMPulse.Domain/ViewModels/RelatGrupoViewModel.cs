using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;

namespace LGMPulse.Domain.ViewModels;

public class RelatGrupoViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }

    public List<GrupoSumary> Grupos { get; set; } = new();
}

public class GrupoSumary
{
    public int IDGrupo {  get; set; }
    public string DescGrupo { get; set; }
    public TipoMovtoEnum TipoMovto { get; set; }
    public decimal ValorGrupo { get; set; }
}
