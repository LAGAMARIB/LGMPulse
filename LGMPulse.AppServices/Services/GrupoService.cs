using LGMDomains.Common;
using LGMDomains.Common.Exceptions;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.Persistence.Interfaces;
using LGMPulse.Persistence.Repositories;

namespace LGMPulse.AppServices.Services;

internal class GrupoService : BaseService<Grupo>, IGrupoService
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IMovtoRepository _movtoRepository;
    private readonly IAgendaRepository _agendaRepository;

    public GrupoService(IGrupoRepository grupoRepository,
                        IMovtoRepository movtoRepository,
                        IAgendaRepository agendaRepository) 
        : base(grupoRepository)
    {
        _grupoRepository = grupoRepository;
        _movtoRepository = movtoRepository;
        _agendaRepository = agendaRepository;
    }

    public async Task<LGMResult<List<Grupo>>> GetListOrderedAsync(Grupo? filterIni = null, Grupo? filterFim = null, string? sortBy = null, List<string>? fields = null)
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

    public override async Task<ILGMResult> DeleteAsync(int? id)
    {
        using (var transCtx = TransactionContext.NewTransaction())
        {
            bool hasMov = await _movtoRepository.ExistsContextualAsync(transCtx, new Movto { IDGrupo = id });
            if (hasMov)
                throw new RuleException("Grupo possui movimentação. Exclusão não permitida.");

            bool hasAgd = await _agendaRepository.ExistsContextualAsync(transCtx, new Agenda { IDGrupo = id });
            if (hasAgd)
                throw new RuleException("Grupo possui agendamento. Exclusão não permitida.");

            Grupo? grupo = await _grupoRepository.GetByIDContextualAsync(transCtx, id);
            if (grupo == null)
                throw new Exception("Registro não disponível para exclusão");

            await _grupoRepository.DeleteContextualAsync(transCtx, grupo);

            return LGMResult.Ok();
        }
    }

}
