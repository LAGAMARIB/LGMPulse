using LGMDomains.Common;
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

    public override async Task<LGMResult<List<Grupo>>> GetListAsync(Grupo? filterIni = null, Grupo? filterFim = null, string? sortBy = null, List<string>? fields = null)
    {
        var lista = await _grupoRepository.GetListAsync(filterIni, filterFim, sortBy, fields);
        var listaOrdenada = lista.OrderByDescending(x => CalcularScore(x)).ThenBy(x => x.Descricao).ToList();
        return LGMResult.Ok(listaOrdenada);
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
