namespace LGMPulse.WebApp.Models;

public class HealthyDashViewModel
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string MesReferencia { get; set; }
    public decimal TotalReceitas { get; set; } = 0;
    public decimal TotalDespesas { get; set; } = 0;
    public decimal PercDiferenca { get; set; } = 0;
    public bool IsMesAtual { get; set; }
}
