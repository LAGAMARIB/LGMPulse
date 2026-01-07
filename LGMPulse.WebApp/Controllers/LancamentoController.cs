using LGMDomains.Common;
using LGMDomains.Common.Helpers;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.Domain.Enuns;
using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace LGMPulse.WebApp.Controllers;

public class LancamentoController : LGMController
{
    private readonly IGrupoService _grupoService;
    private readonly IMovtoService _movtoService;

    public LancamentoController(IGrupoService grupoService, IMovtoService movtoService)
    {
        _grupoService = grupoService;
        _movtoService = movtoService;
    }

    [HttpGet("lancamento/novareceita/{ano=null}/{mes=null}")]
    public async Task<IActionResult> NovaReceita(int? ano=null, int? mes=null)
    {
        return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => GetGruposReceita(ano, mes), "NovaReceita")
        );
    }

    private async Task<LGMResult<NovoLancamentoModel>> GetGruposReceita(int? ano = null, int? mes = null)
    {
        var lista = await _grupoService.GetListAsync(new Grupo { TipoMovto = TipoMovtoEnum.Receita });
        DateTime hoje = DateTimeHelper.Now();
        mes = mes ?? hoje.Month;
        ano = ano ?? hoje.Year;
        NovoLancamentoModel model = new()
        {
            Grupos = lista.Data ?? new(),
            Month = mes ?? hoje.Month,
            Year = ano ?? hoje.Year,
            MesReferencia = DateTimeHelper.MesReferencia(ano!.Value, mes!.Value),
            IsMesAtual = (ano == hoje.Year && mes == hoje.Month),
        };
        return LGMResult.Ok(model);
    }

    [HttpGet("lancamento/novadespesa/{ano=null}/{mes=null}")]
    public async Task<IActionResult> NovaDespesa(int? ano = null, int? mes = null)
    {
        return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => GetGruposDespesa(ano, mes), "NovaDespesa")
        );
    }

    private async Task<LGMResult<NovoLancamentoModel>> GetGruposDespesa(int? ano = null, int? mes = null)
    {
        var lista = await _grupoService.GetListAsync(new Grupo { TipoMovto = TipoMovtoEnum.Despesa });
        DateTime hoje = DateTimeHelper.Now();
        mes = mes ?? hoje.Month;
        ano = ano ?? hoje.Year;
        NovoLancamentoModel model = new()
        {
            Grupos = lista.Data ?? new(),
            Month = mes.Value,
            Year = ano.Value,
            MesReferencia = DateTimeHelper.MesReferencia(ano.Value, mes.Value),
            IsMesAtual = (ano == hoje.Year && mes == hoje.Month),
        };
        return LGMResult.Ok(model);
    }


    [HttpGet("Lancamento/digitarvalor/{tipo}/{idGrupo}/{descricao=null}/{ano=null}/{mes=null}")]
    public IActionResult DigitarValor(TipoMovtoEnum tipo, int idGrupo, string? descricao, int? ano = null, int? mes = null)
    {
        DateTime dataMovto = DateTimeHelper.Now();
        if (ano != null && mes != null && (ano != dataMovto.Year || mes != dataMovto.Month))
        {
            dataMovto = new DateTime(ano.Value, mes.Value, 1);
            dataMovto = dataMovto.AddMonths(1).AddDays(-1); // se mes anterior, lançar no último dia do mês
        }

        DigitarValorViewModel model = new()
        {
            TipoMovto = tipo,
            IDGrupo = idGrupo,
            DescGrupo = descricao ?? "",
            DataMovto = dataMovto,
            MesReferencia = DateTimeHelper.MesReferencia(dataMovto),
            ValorMovto = 0,
            IsNew = true
        };
        return View(model);
    }

    [HttpGet("lancamento/alterarvalor/{IDMovto}/{UrlRetorno=null}")]
    public async Task<IActionResult?> AlterarValorAsync(int IDMovto, string? UrlRetorno=null)
    {
        var result = await _movtoService.GetByIdAsync(IDMovto);
        if (!result.IsSuccess || result.Data == null) return null;

        var movto = result.Data;
        DigitarValorViewModel model = new()
        {
            ID = IDMovto,
            TipoMovto = movto.TipoMovto!.Value,
            IDGrupo = movto.IDGrupo!.Value,
            DescGrupo = movto.NomeGrupo!,
            Descricao = movto.Descricao!,
            DataMovto = movto.DataMovto!.Value,
            MesReferencia = DateTimeHelper.MesReferencia(movto.DataMovto!.Value),
            ValorMovto = movto.ValorMovto!.Value,
            IsNew = false,
            URLRetorno = UrlRetorno
        };
        return View("DigitarValor", model);
    }

    [HttpPost("Lancamento/save")]
    public async Task<JsonResult> Save([FromBody] DigitarValorViewModel model)
    {
        Movto movto = new()
        {
            ID = model.ID,
            DataMovto = model.DataMovto,
            TipoMovto = model.TipoMovto,
            IDGrupo = model.IDGrupo,
            Descricao = model.Descricao,
            ValorMovto= model.ValorMovto
        };
        ILGMResult result;
        if (model.IsNew)
            result = await _movtoService.CreateAsync(movto);
        else
            result = await _movtoService.UpdateAsync(movto);
        if (result.IsSuccess)
        {
            if (!string.IsNullOrEmpty(result.Message))
                GravarAviso(result.Message);
            else
                GravarMensagem("Registro salvo com sucesso");
            if (!string.IsNullOrWhiteSpace(model.URLRetorno))
                result.RedirectUrl = model.URLRetorno.Replace('_','/');
        }
        return Json(result);
    }

    [HttpPost("lancamento/delete/{IDMovto}")]
    public async Task<JsonResult> Delete(int IDMovto)
    {
        var result = await _movtoService.DeleteAsync(IDMovto);
        GravarMensagem("Movimento excluído com sucesso");
        return Json(result);
    }
}
