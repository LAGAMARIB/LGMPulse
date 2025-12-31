namespace LGMPulse.Domain.ViewModels;

public class RelatEvolucaoViewModel
{
    public List<EvolucaoSumary> Receitas { get; set; } = new();
    public List<EvolucaoSumary> Despesas { get; set; } = new();
    public List<EvolucaoSumary> Liquidez { get; set; } = new();
    public decimal ValMaxRecDesp { get; set; } = 1;
}

public class EvolucaoSumary
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MesReferencia { get; set; }
    public decimal ValorTotal { get; set; }
}
