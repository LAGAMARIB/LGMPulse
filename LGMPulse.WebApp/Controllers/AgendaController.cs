using LGMDomains.Common;
using LGMDomains.Common.Helpers;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;
using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LGMPulse.WebApp.Controllers;

public class AgendaController : LGMController
{

    [HttpGet("agenda/{ano=null}/{mes=null}")]
    public async Task<IActionResult> Agenda(int? ano = null, int? mes = null)
    {
        return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => getAgendaViewModelAsync(ano, mes))
        );
    }

    private async Task<LGMResult<AgendaViewModel>> getAgendaViewModelAsync(int? ano=null, int? mes=null, int? dia=null)
    {
        var startDate = DateTimeHelper.Now().Date;
        if (ano != null && mes != null)
            startDate = new DateTime(ano!.Value, mes!.Value, 01);
        startDate = new DateTime(startDate.Year, startDate.Month, 01);
        var endDate = startDate.AddMonths(1).AddSeconds(-1);

        AgendaViewModel viewModel = new();
        viewModel.Date = startDate;
        viewModel.IsOnlyOneDay = (dia != null);
        if (viewModel.IsOnlyOneDay)
            endDate = endDate.Date.AddDays(1).AddSeconds(-1);

        // pegar vencidos de meses anteriores
        DateTime deteDelay = (viewModel.IsOnlyOneDay ? startDate : startDate.AddYears(-1));

        //LGMResult<List<Agenda>> result = await _agendaService.GetListAsync(
        //                new Agenda { DataVencto = deteDelay, StatusParcela = ParcelaStatusEnum.Pendente },
        //                new Agenda { DataVencto = endDate, StatusParcela = ParcelaStatusEnum.Pendente });

        LGMResult<List<Agenda>> result = GetListMock(
                        new Agenda { DataVencto = deteDelay, StatusParcela = ParcelaStatusEnum.Pendente },
                        new Agenda { DataVencto = endDate, StatusParcela = ParcelaStatusEnum.Pendente });

        if (!result.IsSuccess)
            return LGMResult.Fail<AgendaViewModel>(result.Message);

        var listAgenda = result.Data ?? new();

        // marcar atrasados mas enviar apenas os agendamentos do periodo selecionado
        viewModel.HasDelayed = listAgenda.Any((x) => x.DataVencto!.Value < startDate);
        viewModel.Agendas = listAgenda.Where(x => x.DataVencto!.Value >= startDate).ToList() ?? new();


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



    private LGMResult<List<Agenda>> GetListMock(Agenda filterIni, Agenda filterFim)
    {
        List<Agenda> lista = new();
        var agora = DateTimeHelper.Now();
        lista.Add(new Agenda() { ID=1, DataMovto=agora, DataVencto=agora.AddDays(-3), TipoMovto=TipoMovtoEnum.Despesa, IDGrupo=5, NomeGrupo="Carro", Parcela=1, ValorParcela=100.00m });
        lista.Add(new Agenda() { ID=1, DataMovto=agora, DataVencto=agora, TipoMovto=TipoMovtoEnum.Despesa, IDGrupo=6, NomeGrupo="Moradia", Parcela=1, ValorParcela=800.00m });
        lista.Add(new Agenda() { ID=1, DataMovto=agora, DataVencto=agora.AddDays(1), TipoMovto=TipoMovtoEnum.Despesa, IDGrupo=7, NomeGrupo="Saúde", Parcela=1, ValorParcela=333.67m });
        lista.Add(new Agenda() { ID=1, DataMovto=agora, DataVencto=agora.AddDays(2), TipoMovto= TipoMovtoEnum.Despesa, IDGrupo=7, NomeGrupo="Saúde", Parcela=1, ValorParcela=450.00m });
        lista.Add(new Agenda() { ID=1, DataMovto=agora, DataVencto=agora.AddDays(5), TipoMovto=TipoMovtoEnum.Despesa, IDGrupo=8, NomeGrupo="Educação", Parcela =1, ValorParcela=240.55m });
        return LGMResult.Ok(lista);
    }
}
