using LGMPulse.Domain.Domains;

namespace LGMPulse.WebApp.Models;

public class NovoLancamentoModel
{
    public int Year { get; set; }
    public int Month { get; set; }

    public List<Grupo> Grupos { get; set; } = new();
    public string MesReferencia { get; set; }
}
