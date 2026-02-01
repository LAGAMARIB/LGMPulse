using LGMPulse.Domain.Domains;

namespace LGMPulse.WebApp.Models;

public class EditGrupoModel
{
    public List<string> IconsPath { get; set; } = new();
    public Grupo Grupo { get; set; }
}
