using LGMPulse.Domain.Enuns;

namespace LGMPulse.WebApp.Models;

public class GrupoModel
{
    public int? ID { get; set; }
    public string Descricao { get; set; }
    public TipoMovtoEnum TipoMovto { get; set; }
    public string ImagePath { get; set; }
}
