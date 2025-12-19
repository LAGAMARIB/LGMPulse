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

    public async Task<IActionResult> NovaReceita()
    {
        return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => GetGruposReceita(), "NovaReceita")
        );
    }

    private async Task<LGMResult<List<Grupo>>> GetGruposReceita()
    {
        var lista = await _grupoService.GetListAsync(new Grupo { TipoMovto = TipoMovtoEnum.Receita });
        return lista;
    }

    public async Task<IActionResult> NovaDespesa()
    {
        return await ValidateSessionAsync(() =>
                ExecuteViewAsync(() => GetGruposDespesa(), "NovaDespesa")
        );
    }

    private async Task<LGMResult<List<Grupo>>> GetGruposDespesa()
    {
        var lista = await _grupoService.GetListAsync(new Grupo { TipoMovto = TipoMovtoEnum.Despesa });
        return lista;
    }


    [HttpGet("Lancamento/digitarvalor/{tipo}/{idGrupo}/{descricao=null}")]
    public IActionResult DigitarValor(TipoMovtoEnum tipo, int idGrupo, string? descricao)
    {
        DigitarValorViewModel model = new()
        {
            TipoMovto = tipo,
            IDGrupo = idGrupo,
            DescGrupo = descricao ?? "",
            DataMovto = DateTimeHelper.Now(),
            ValorMovto = 0
        };
        return View(model);
    }

    [HttpPost("Lancamento/create")]
    public async Task<JsonResult> Create([FromBody] DigitarValorViewModel model)
    {
        Movto movto = new()
        {
            DataMovto = model.DataMovto,
            TipoMovto = model.TipoMovto,
            IDGrupo = model.IDGrupo,
            Descricao = model.Descricao,
            ValorMovto= model.ValorMovto
        };
        var result = await _movtoService.CreateAsync(movto);
        if (result.IsSuccess)
        {
            if (!string.IsNullOrEmpty(result.Message))
                GravarAviso(result.Message);
            else
                GravarMensagem("Registro salvo com sucesso");
        }
        return Json(result);
    }

}
