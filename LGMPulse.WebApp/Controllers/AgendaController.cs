using LGMDomains.Common;
using LGMDomains.Common.Helpers;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;
using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace LGMPulse.WebApp.Controllers;

public class AgendaController : LGMController
{
    private readonly IGrupoService _grupoService;
    private readonly IAgendaService _agendaService;

    public AgendaController(IGrupoService grupoService, IAgendaService agendaService)
    {
        _grupoService = grupoService;
        _agendaService = agendaService;
    }

    [HttpGet("agenda/{ano=null}/{mes=null}")]
    public async Task<IActionResult> Agenda(int? ano=null, int? mes=null)
    {
        var hoje = DateTimeHelper.Now();
        ano ??= hoje.Year;
        mes ??= hoje.Month;
        return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => getAgendaViewModelAsync(ano!.Value, mes!.Value))
        );
    }

    private async Task<LGMResult<AgendaViewModel>> getAgendaViewModelAsync(int ano, int mes, int? dia=null)
    {
        var hoje = DateTimeHelper.Now().Date;
        var startDate = new DateTime(ano, mes, 1);
        var endDate = startDate.AddMonths(1).AddSeconds(-1);

        AgendaViewModel viewModel = new();
        viewModel.IsOnlyOneDay = (dia != null);
        if (viewModel.IsOnlyOneDay)
        {
            startDate = new DateTime(ano, mes, dia!.Value);
            endDate = startDate.AddDays(1).AddSeconds(-1);
        }
        viewModel.Date = startDate;

        // pegar vencidos de meses anteriores
        DateTime deteDelay = (viewModel.IsOnlyOneDay ? startDate : startDate.AddYears(-1));

        LGMResult<List<Agenda>> result = await _agendaService.GetListAsync(
                        new Agenda { DataVencto = deteDelay, StatusParcela = ParcelaStatusEnum.Pendente },
                        new Agenda { DataVencto = endDate, StatusParcela = ParcelaStatusEnum.Pendente });

        if (!result.IsSuccess)
            return LGMResult.Fail<AgendaViewModel>(result.Message);

        var listAgenda = result.Data ?? new();

        // marcar atrasados mas enviar apenas os agendamentos do periodo selecionado
        viewModel.HasDelayed = listAgenda.Any((x) => x.DataVencto!.Value < startDate);
        viewModel.Agendas = listAgenda.Where(x => x.DataVencto!.Value >= startDate).OrderBy(x => x.DataVencto).ToList() ?? new();

        return  LGMResult.Ok(viewModel);
    }

    [HttpGet("agenda/gradeagenda/dia/{date=null}")]
    public async Task<IActionResult> GradeAgendaAsync(DateTime? date = null)
    {
        date ??= DateTimeHelper.Now().Date;
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync(() => getAgendaViewModelAsync(date.Value.Year, date.Value.Month, date.Value.Day), "GradeAgenda")
        );
    }

    [HttpGet("agenda/gradeagenda/mes/{month}/{year}")]
    public async Task<IActionResult> GradeAgendaAsync(int month, int year)
    {
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync(() => getAgendaViewModelAsync(year, month), "GradeAgenda")
        );
    }

    [HttpGet("agenda/novareceita/{dataLancto}")]
    public async Task<IActionResult> NovaReceita(DateTime dataLancto)
    {
        return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => GetGruposReceita(dataLancto), "NovaReceita")
        );
    }

    private async Task<LGMResult<NovoLancamentoModel>> GetGruposReceita(DateTime dataLancto)
    {
        var lista = await _grupoService.GetListAsync(new Grupo { TipoMovto = TipoMovtoEnum.Receita });
        var hoje = DateTimeHelper.Now();
        NovoLancamentoModel model = new()
        {
            IsAgenda = true,
            Grupos = lista.Data ?? new(),
            DateLancto = dataLancto,
            MesReferencia = DateTimeHelper.MesReferencia(dataLancto),
            IsMesAtual = (dataLancto.Year == hoje.Year && dataLancto.Month == hoje.Month),
        };
        return LGMResult.Ok(model);
    }

    [HttpGet("agenda/novadespesa/{dataLancto}")]
    public async Task<IActionResult> NovaDespesa(DateTime dataLancto)
    {
        return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => GetGruposDespesa(dataLancto), "NovaDespesa")
        );
    }

    private async Task<LGMResult<NovoLancamentoModel>> GetGruposDespesa(DateTime dataLancto)
    {
        var lista = await _grupoService.GetListAsync(new Grupo { TipoMovto = TipoMovtoEnum.Despesa });
        DateTime hoje = DateTimeHelper.Now();
        NovoLancamentoModel model = new()
        {
            IsAgenda = true,
            Grupos = lista.Data ?? new(),
            DateLancto = dataLancto,
            MesReferencia = DateTimeHelper.MesReferencia(dataLancto),
            IsMesAtual = (dataLancto.Year == hoje.Year && dataLancto.Month == hoje.Month),
        };
        return LGMResult.Ok(model);
    }

    [HttpGet("agenda/editar/{IDMovto}/{UrlRetorno=null}")]
    public async Task<IActionResult?> EditarAsync(int IDMovto, string? UrlRetorno = null)
    {
        var result = await _agendaService.GetByIdAsync(IDMovto);
        if (!result.IsSuccess || result.Data == null) return null;

        var agenda = result.Data;
        DigitarValorViewModel model = new()
        {
            IsAgenda = true,
            ID = IDMovto,
            TipoMovto = agenda.TipoMovto!.Value,
            IDGrupo = agenda.IDGrupo!.Value,
            DescGrupo = agenda.NomeGrupo!,
            Descricao = agenda.Descricao!,
            DataMovto = agenda.DataVencto!.Value,
            MesReferencia = DateTimeHelper.MesReferencia(agenda.DataVencto!.Value),
            ValorMovto = agenda.ValorParcela!.Value,
            QtdParcelas = agenda.QtdParcelas!.Value,
            Parcela = agenda.Parcela!.Value,
            StatusParcela = agenda.StatusParcela!.Value,
            Recorrente = agenda.Recorrente!.Value,
            IsNew = false,
            URLRetorno = UrlRetorno
        };
        return View("DigitarValor", model);
    }

    [HttpPost("agenda/save")]
    public async Task<JsonResult> Save([FromBody] DigitarValorViewModel model)
    {
        Agenda agenda = new()
        {
            ID = model.ID,
            DataMovto = DateTimeHelper.Now(),
            DataVencto = model.DataMovto,
            TipoMovto = model.TipoMovto,
            IDGrupo = model.IDGrupo,
            Descricao = model.Descricao,
            ValorParcela = model.ValorMovto,
            QtdParcelas = model.QtdParcelas,
            Parcela = model.Parcela,
            StatusParcela = model.StatusParcela,
            Recorrente = model.Recorrente
        };
        ILGMResult result;
        if (model.IsNew)
            result = await _agendaService.CreateAsync(agenda);
        else
            result = await _agendaService.UpdateAsync(agenda, [nameof(agenda.DataMovto), nameof(agenda.Descricao), nameof(agenda.ValorParcela)]);
        if (result.IsSuccess)
        {
            if (!string.IsNullOrEmpty(result.Message))
                GravarAviso(result.Message);
            else
                GravarMensagem("Registro salvo com sucesso");
            if (!string.IsNullOrWhiteSpace(model.URLRetorno))
                result.RedirectUrl = model.URLRetorno.Replace('_', '/');
        }
        return Json(result);
    }

    [HttpPost("agenda/delete/{IDMovto}")]
    public async Task<JsonResult> Delete(int IDMovto)
    {
        var result = await _agendaService.DeleteAsync(IDMovto);
        GravarMensagem("Movimento excluído com sucesso");
        return Json(result);
    }

}
