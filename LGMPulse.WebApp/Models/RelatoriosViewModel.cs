namespace LGMPulse.WebApp.Models;

public class RelatoriosViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MesReferencia { get; set; } = "";
    public bool IsMesAtual { get; set; }
    public bool IsFreeMode { get; set; }

}
