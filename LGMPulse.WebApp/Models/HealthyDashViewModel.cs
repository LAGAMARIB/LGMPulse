namespace LGMPulse.WebApp.Models;

public class HealthyDashViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalReceitas { get; set; } = 0;
    public decimal TotalDespesas { get; set; } = 0;
    public decimal PercDiferenca { get; set; } = 0;
    public bool IsFreeMode { get; set; }

}
