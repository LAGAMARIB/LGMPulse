using LGMPulse.Domain.Enuns;

namespace LGMPulse.WebApp.Models;

public class DigitarValorViewModel
{
    public TipoMovtoEnum TipoMovto { get; set; }
    public int IDGrupo { get; set; }
    public string DescGrupo { get; set; } = string.Empty;
    public DateTime DataMovto { get; set; }
    public string? Descricao { get; set; }
    public decimal ValorMovto { get; set; }
    public string MesReferencia { get; set; }
}
