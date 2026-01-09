using LGMPulse.Domain.Domains;

namespace LGMPulse.WebApp.Models;

internal class AgendaViewModel
{
    public DateTime Date { get; set; }
    public bool IsOnlyOneDay { get; set; }
    public bool HasDelayed { get; set; }
    public int Month => Date.Month;
    public int Year => Date.Year;
    public int Day => Date.Day;

    public List<Agenda> Agendas { get; set; } = new();
}
