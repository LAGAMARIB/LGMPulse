using LGMPulse.Domain.Domains;

namespace LGMPulse.WebApp.Models;

public class ExtratoViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<Movto> Movtos { get; set; }
    public string MovtoReferencia { get; set; } = "";
}
