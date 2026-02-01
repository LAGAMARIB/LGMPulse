using LGMDomains.Common;
using LGMPulse.AppServices.Interfaces;
using LGMPulse.Domain.Domains;
using LGMPulse.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;

namespace LGMPulse.WebApp.Controllers;

public class GrupoController : LGMController
{
    private readonly IGrupoService _grupoService;
    private readonly IWebHostEnvironment _env;

    public GrupoController(IGrupoService grupoService, IWebHostEnvironment env)
    {
        _grupoService = grupoService;
        _env = env;
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
            ExecuteViewAsync(() => newEditGrupo(), "EditGrupo")
        );
    }


    [HttpGet("grupo/edit/{id}")]
    public async Task<IActionResult> EditGrupo(int id)
    {
        return await ValidateSessionAsync(() =>
            ExecuteViewAsync(() => getEditGrupo(id))
        );
    }

    private Task<LGMResult<EditGrupoModel>> newEditGrupo()
    {
        EditGrupoModel model = new()
        {
            Grupo = new()
        };
        model.IconsPath = getIconsPaths();
        model.Grupo.ImagePath = "/icons/plus-misc.svg";
        return Task.FromResult( LGMResult.Ok(model) );
    }

    private async Task<LGMResult<EditGrupoModel>> getEditGrupo(int id)
    {
        var result = await _grupoService.GetByIdAsync(id);
        EditGrupoModel model = new()
        {
            Grupo = result.Data ?? new()
        };
        model.IconsPath = getIconsPaths();
        return LGMResult.Ok(model);
    }

    private List<string> getIconsPaths()
    {
        var result = new List<string>();

        var iconsFolder = Path.Combine(_env.WebRootPath, "icons");

        if (!Directory.Exists(iconsFolder))
            return result;

        var files = Directory.GetFiles(iconsFolder, "*.svg");

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            result.Add($"/icons/{fileName}");
        }

        return result;
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
