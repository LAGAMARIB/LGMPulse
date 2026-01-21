using LGMPulse.Domain.Enuns;

namespace LGMPulse.Domain.ViewModels;

public class MapaFinanceiroViewModel
{
    public int Year { get; set; }
    public int LastMonth { get; set; }
    public MapaModel Receitas { get; set; }
    public MapaModel Despesas { get; set; }

    public List<MapaModel> Mapas { get; set; }
}

public class MapaModel
{
    public int? IDGrupo { get; set; }
    public TipoMovtoEnum? TipoMovto { get; set; }  // int 0-Receita 1-Despesa
    public string? DescGrupo { get; set; }
    public decimal[] TotalMes { get; set; } = new decimal[14];
}