using LGMDomains.Common;
using LGMDomains.Common.Exceptions;
using LGMDomains.Common.Helpers;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;
using LGMPulse.Domain.ViewModels;
using LGMPulse.Persistence.Interfaces;
using LGMPulse.Persistence.Repositories;

namespace LGMPulse.AppServices.Services;

internal class AgendaService : BaseService<Agenda>, IAgendaService
{
    private readonly IAgendaRepository _agendaRepository;
    private readonly IMovtoRepository _movtoRepository;

    public AgendaService(IAgendaRepository agendaRepository, IMovtoRepository movtoRepository) : base(agendaRepository)
    {
        _agendaRepository = agendaRepository;
        _movtoRepository = movtoRepository;
    }

    public override async Task<ILGMResult> CreateAsync(Agenda agenda)
    {
        if (agenda.QtdParcelas <= 0)
            agenda.QtdParcelas = 1;

        if (agenda.Parcela > agenda.QtdParcelas)
            throw new RuleException($"Quantidade de parcelas ({agenda.QtdParcelas}) inferior a parcela a ser criada ({agenda.Parcela}). Operação não permitida.");
        if (agenda.ValorParcela is null || agenda.Parcela is null || agenda.Parcela <= 0)
            throw new RuleException("Valor da parcela deve ser maior que zero.");

        // parcelas recorrentes
        if (agenda.Recorrente == true && agenda.QtdParcelas > agenda.Parcela)
        {
            var idRecorrencia = DateTimeHelper.Now().Ticks.ToString();
            var dataVencto = agenda.DataVencto!.Value;
            using (var transCtx = TransactionContext.NewTransaction())
            {
                for (var parc = (int)agenda.Parcela; parc <= agenda.QtdParcelas; parc++)
                {
                    Agenda newAgenda = agenda.Clone<Agenda>();
                    newAgenda.Parcela = parc;
                    newAgenda.IDRecorrencia = idRecorrencia;
                    newAgenda.DataVencto = dataVencto;
                    await _agendaRepository.CreateTransactionalAsync(transCtx, newAgenda, null);
                    dataVencto = agenda.DataVencto.Value.AddMonths(parc);
                }

                var transResult = await transCtx.ExecuteTransactionAsync();
                if (!transResult)
                    throw new Exception("Falha geral na criação das parcelas");
                return LGMResult.Ok();
            }
        }

        // única parcela
        var newId = await _agendaRepository.CreateAsync(agenda);
        return LGMResult.Ok(newId);
    }

    //PayoffAsync
    public async Task<ILGMResult> BaixarAsync(int id)
    {
        using (var transCtx = TransactionContext.NewTransaction())
        {
            var agenda = await _agendaRepository.GetByIDContextualAsync(transCtx, id);
            if (agenda == null)
                throw new RuleException("Registro não disponível");
            if (agenda.StatusParcela != ParcelaStatusEnum.Pendente)
                throw new RuleException("Parcela já quitada. Transação não permitida.");

            agenda.StatusParcela = ParcelaStatusEnum.Quitada;
            var descMovo = string.IsNullOrWhiteSpace(agenda.Descricao) ? agenda.NomeGrupo : agenda.Descricao;
            if (agenda.QtdParcelas > 1)
                descMovo += $" {agenda.Parcela}/{agenda.QtdParcelas}";

            Movto movto = new()
            {
                DataMovto = DateTimeHelper.Now(),
                Descricao = descMovo,
                IDAgenda = agenda.ID,
                IDGrupo = agenda.IDGrupo,
                TipoMovto = agenda.TipoMovto,
                ValorMovto = agenda.ValorParcela
            };

            await _movtoRepository.CreateTransactionalAsync(transCtx, movto);
            await _agendaRepository.UpdateTransactionalAsync(transCtx, agenda, [nameof(agenda.StatusParcela)]);

            if (!await transCtx.ExecuteTransactionAsync())
                throw new Exception("Falha geral na execução da tarefa");

            return LGMResult.Ok();
        }
    }

    public override async Task<ILGMResult> DeleteAsync(int? id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));
        using (var transCtx = TransactionContext.NewTransaction())
        {
            var agenda = await _agendaRepository.GetByIDContextualAsync(transCtx, id);
            if (agenda == null)
                throw new RuleException("Registro não disponível para exclusão");
            if (agenda.StatusParcela != ParcelaStatusEnum.Pendente)
                throw new RuleException("Parcela já quitada. Exclusão não permitida.");
            if (agenda.Recorrente == true && !string.IsNullOrEmpty(agenda.IDRecorrencia))
            {
                var filter = new Agenda { IDRecorrencia = agenda.IDRecorrencia };
                var deleteList = await _agendaRepository.GetListContextualAsync(transCtx, filter, null, null, null);
                foreach (var item in deleteList)
                {
                    if (item.Parcela >= agenda.Parcela && item.StatusParcela == ParcelaStatusEnum.Pendente)
                        await _agendaRepository.DeleteTransactionalAsync(transCtx, item);
                    else
                    {
                        item.QtdParcelas = (agenda.Parcela - 1);
                        await _agendaRepository.UpdateTransactionalAsync(transCtx, item, [nameof(agenda.QtdParcelas)]);
                    }
                }

                if (deleteList.Count > 0)
                    await transCtx.ExecuteTransactionAsync();
            }
            else
            {
                await _agendaRepository.DeleteContextualAsync(transCtx, agenda);
            }
            return LGMResult.Ok();
        }
    }

    public async Task<ILGMResult> UpdateAgendaAsync(Agenda agenda, bool applyToAll)
    {
    //result = await _agendaService.UpdateAgendaAsync(agenda, [nameof(agenda.DataMovto), nameof(agenda.Descricao), nameof(agenda.ValorParcela)]);
        if (agenda.StatusParcela != ParcelaStatusEnum.Pendente)
            return LGMResult.Fail("Parcela já quitada. Operação não permitida.");

        List<string> campos = [nameof(agenda.DataMovto), nameof(agenda.Descricao), nameof(agenda.ValorParcela)];
        if (agenda.Recorrente == true && !string.IsNullOrEmpty(agenda.IDRecorrencia) && applyToAll)
        {
            using (var transCtx = TransactionContext.NewTransaction())
            {
                var filter = new Agenda { IDRecorrencia = agenda.IDRecorrencia, StatusParcela = ParcelaStatusEnum.Pendente };
                var updateList = await _agendaRepository.GetListContextualAsync(transCtx, filter, null, null, null);
                foreach (var item in updateList)
                {
                    if (item.Parcela >= agenda.Parcela)
                    {
                        AlterarCampos(origem: agenda, destino: item, campos: campos);
                        await _agendaRepository.UpdateTransactionalAsync(transCtx, item, campos);
                    }
                }

                if (updateList.Count > 0)
                    await transCtx.ExecuteTransactionAsync();
            }
        }
        else
        {
            await _agendaRepository.UpdateAsync(agenda, campos);
        }
        return LGMResult.Ok();
    }

    private void AlterarCampos(Agenda origem, Agenda destino, List<string>? campos)
    {
        if (campos == null || campos.Count == 0)
            return;

        foreach (var nomeCampo in campos)
        {
            var propOrigem = typeof(Agenda).GetProperty(nomeCampo);
            var propDestino = typeof(Agenda).GetProperty(nomeCampo);

            if (propOrigem != null && propDestino != null && propDestino.CanWrite)
            {
                var valor = propOrigem.GetValue(origem);
                propDestino.SetValue(destino, valor);
            }
        }
    }

}
