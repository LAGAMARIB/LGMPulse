using LGMDomains.Common;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace LGMPulse.WebApp.Controllers;

public class GrupoController : LGMController
{
    private readonly IGrupoService _grupoService;

    public GrupoController(IGrupoService grupoService)
    {
        _grupoService = grupoService;
    }

    [HttpGet("grupos")]
    public async Task<IActionResult> Grupos()
    {
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync(() => getListGrupos())
        );
    }

    private async Task<LGMResult<List<Grupo>>> getListGrupos()
    {
        return await _grupoService.GetListAsync(null, null, nameof(Grupo.Descricao));
    }

    [HttpGet("grupo/create")]
    public async Task<IActionResult> CreateGrupo()
    {
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync(() => newGrupo(), "EditGrupo")
        );
    }

    private Task<LGMResult<Grupo>> newGrupo() {
        return Task.FromResult( LGMResult.Ok(new Grupo()) );
    }

    [HttpGet("grupo/edit/{id}")]
    public async Task<IActionResult> EditGrupo(int id)
    {
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync(() => getGrupo(id))
        );
    }

    private async Task<LGMResult<Grupo>> getGrupo(int id)
    {
        var result = await _grupoService.GetByIdAsync(id);
        var grupo = result.Data ?? new();
        return LGMResult.Ok(grupo);
    }

    [HttpPost("grupo/save")]
    public async Task<JsonResult> Save(GrupoModel grupoModel)
    {
        Grupo grupo = new()
        {
            ID = grupoModel.ID,
            Descricao = grupoModel.Descricao,
            TipoMovto = grupoModel.TipoMovto,
            ImagePath = grupoModel.ImagePath
        };
        
        ILGMResult result;
        if (grupoModel.ID == null)
            result = await _grupoService.CreateAsync(grupo);
        else
            result = await _grupoService.UpdateAsync(grupo, [nameof(grupo.Descricao), nameof(grupo.ImagePath)]);
        
        if (result.IsSuccess)
        {
            if (!string.IsNullOrEmpty(result.Message))
                GravarAviso(result.Message);
            else
                GravarMensagem("Registro salvo com sucesso");
        }
        return Json(result);
    }

    [HttpPost("grupo/delete/{Id}")]
    public async Task<JsonResult> DeleteAsync(int Id)
    {
        var result = await _grupoService.DeleteAsync(Id);
        GravarMensagem("Grupo excluído com sucesso");
        return Json(result);
    }
}
