using LGMDomains.Common;
using LGMDomains.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net;

namespace LGMPulse.WebApp.Filters;

public class LGMExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger<LGMExceptionFilter> _logger;
    private readonly ITempDataDictionaryFactory _tempDataFactory;

    public LGMExceptionFilter(ILogger<LGMExceptionFilter> logger, ITempDataDictionaryFactory tempDataFactory)
    {
        _logger = logger;
        _tempDataFactory = tempDataFactory;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var exception = context.Exception;
        _logger.LogError(exception, "Erro não tratado capturado pelo filtro global.");

        // Detecta se é uma chamada AJAX (padrão X-Requested-With) ou se o action retorna JsonResult
        bool isAjax = string.Equals(context.HttpContext.Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

        var actionDescriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        var returnType = actionDescriptor?.MethodInfo?.ReturnType;

        bool expectsJson = false;
        if (returnType != null)
        {
            if (typeof(JsonResult).IsAssignableFrom(returnType))
                expectsJson = true;
            else
            {
                // métodos async que retornam Task<JsonResult>
                if (returnType.IsGenericType && typeof(Task).IsAssignableFrom(returnType))
                {
                    var gen = returnType.GetGenericArguments().FirstOrDefault();
                    if (gen != null && (typeof(JsonResult).IsAssignableFrom(gen) || gen.Name.Contains("JsonResult")))
                        expectsJson = true;
                }
            }
        }

        // Se header ajax ou espera json explicitamente
        if (isAjax || expectsJson)
        {
            HandleJsonException(context, exception);
        }
        else
        {
            HandleViewException(context, exception);
        }

        context.ExceptionHandled = true;
        await Task.CompletedTask;
    }

    private void HandleJsonException(ExceptionContext context, Exception exception)
    {
        ILGMResult result;

        if (exception is RuleException ruleEx)
            result = LGMResult.Fail(ruleEx.Message);
        else if (exception is UnauthorizedAccessException)
            result = LGMResult.Fail("Acesso não autorizado. Efetue login novamente.");
        else
            result = LGMResult.Fail("Ocorreu um erro inesperado. " + exception.Message);

        context.Result = new JsonResult(result)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }

    private void HandleViewException(ExceptionContext context, Exception exception)
    {
        if (exception is UnauthorizedAccessException)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;
        }

        // Cria um ViewDataDictionary independente (não precisamos do Controller)
        var viewData = new ViewDataDictionary(new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(), context.ModelState)
        {
            ["Mensagem"] = exception.Message
        };

        // Se quiser ainda usar TempData (ex.: exibir toast) podemos setar:
        try
        {
            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            tempData["Erro"] = exception.Message;
        }
        catch
        {
            // ignore se não for possível popular TempData
        }

        context.Result = new ViewResult
        {
            ViewName = "ViewError",
            StatusCode = (int)HttpStatusCode.InternalServerError,
            ViewData = viewData
        };
    }
}
