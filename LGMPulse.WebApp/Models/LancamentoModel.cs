using LGMPulse.Domain.Enuns;

namespace LGMPulse.WebApp.Models;

public class LancamentoModel
{
    public int? ID { get; set; }
    public TipoMovtoEnum TipoMovto { get; set; }
    public int IDGrupo { get; set; }
    public string DescGrupo { get; set; } = string.Empty;
    public DateTime DataMovto { get; set; }
    public string? Descricao { get; set; }
    public decimal ValorMovto { get; set; }
    public string MesReferencia { get; set; }
    public bool IsNew => (ID == null || ID == 0);
    public string? URLRetorno { get; set; }
    public bool IsAgenda { get; set; }
    public int QtdParcelas { get; set; } = 1;
    public int Parcela { get; set; } = 1;
    public ParcelaStatusEnum StatusParcela { get; set; } = ParcelaStatusEnum.Pendente;
    public bool Recorrente { get; set; }
    public string IDRecorrencia { get; set; }
    public bool UpdateAll { get; set; }
}
