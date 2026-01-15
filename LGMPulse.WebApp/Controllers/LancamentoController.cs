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

    [HttpGet("lancamento/novareceita/{dataLancto}")]
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
            IsAgenda = false,
            Grupos = lista.Data ?? new(),
            DateLancto = dataLancto,
            MesReferencia = DateTimeHelper.MesReferencia(dataLancto),
            IsMesAtual = (dataLancto.Year == hoje.Year && dataLancto.Month == hoje.Month),
        };
        return LGMResult.Ok(model);
    }

    [HttpGet("lancamento/novadespesa/{dataLancto}")]
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
            IsAgenda = false,
            Grupos = lista.Data ?? new(),
            DateLancto = dataLancto,
            MesReferencia = DateTimeHelper.MesReferencia(dataLancto),
            IsMesAtual = (dataLancto.Year == hoje.Year && dataLancto.Month == hoje.Month),
        };
        return LGMResult.Ok(model);
    }


    [HttpGet("Lancamento/digitarvalor/{descricao=null}")]
    public IActionResult DigitarValor(string? descricao=null)
    {
        DigitarValorModel model = new()
        {
            Descricao = descricao ?? "",
        };
        return View(model);
    }

    [HttpGet("lancamento/alterarvalor/{descricao}/{valor}")]
    public async Task<IActionResult> AlterarValorAsync(string descricao, decimal valor)
    {
        DigitarValorModel model = new()
        {
            Descricao = descricao,
            ValorInicial = valor,
        };
        return View("DigitarValor", model);
    }

    [HttpPost("Lancamento/save")]
    public async Task<JsonResult> Save([FromBody] LancamentoModel model)
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

    [HttpGet("lancamento/getmovto/{IDMovto}")]
    public async Task<JsonResult> GetMovtoAsync(int IDMovto)
    {
        var result = await _movtoService.GetByIdAsync(IDMovto);
        var movto = result.Data;
        return Json(movto);
    }

}
