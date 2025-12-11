using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.Persistence.Interfaces;

namespace LGMPulse.AppServices.Services;

internal class GrupoService : BaseService<Grupo>, IGrupoService
{
    private readonly IGrupoRepository _grupoRepository;

    public GrupoService(IGrupoRepository grupoRepository) 
        : base(grupoRepository)
    {
        _grupoRepository = grupoRepository;
    }

    private double CalcularScore(Grupo grupo)
    {
        int qtd = grupo.QtdMovtos ?? 0;
        DateTime ultima = grupo.DateUltMovto ?? DateTime.MinValue;

        var dias = (DateTime.Today - ultima).TotalDays;

        var freqScore = Math.Log10(qtd + 1);
        var recentScore = Math.Exp(-dias / 7.0);

        double score = recentScore * 0.7 + freqScore * 0.3;

        return score;
    }

}
