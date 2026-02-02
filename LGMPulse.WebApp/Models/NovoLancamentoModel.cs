using LGMPulse.Domain.Domains;

namespace LGMPulse.WebApp.Models;

public class NovoLancamentoModel
{
    public DateTime DateLancto { get; set; }
    public int Year => DateLancto.Year;
    public int Month => DateLancto.Month;
    public int Day => DateLancto.Day;

    public List<Grupo> Grupos { get; set; } = new();
    public string MesReferencia { get; set; }
    public bool IsMesAtual { get; set; }
    public bool IsAgenda { get; set; }
    public bool IsFreeMode { get; set; }
}
