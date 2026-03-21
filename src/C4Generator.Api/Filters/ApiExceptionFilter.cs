using C4Generator.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ValidationException = C4Generator.Application.Exceptions.ValidationException;

namespace C4Generator.Api.Filters;

public sealed class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case NotFoundException ex:
                context.Result = new NotFoundObjectResult(new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound,
                    Instance = context.HttpContext.Request.Path
                });
                context.ExceptionHandled = true;
                break;

            case ConflictException ex:
                context.Result = new ConflictObjectResult(new ProblemDetails
                {
                    Title = "Conflict",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict,
                    Instance = context.HttpContext.Request.Path
                });
                context.ExceptionHandled = true;
                break;

            case UnauthorizedException ex:
                context.Result = new UnauthorizedObjectResult(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = ex.Message,
                    Status = StatusCodes.Status401Unauthorized,
                    Instance = context.HttpContext.Request.Path
                });
                context.ExceptionHandled = true;
                break;

            case ValidationException ex:
                context.Result = new UnprocessableEntityObjectResult(new ValidationProblemDetails(
                    ex.Errors.ToDictionary(kv => kv.Key, kv => kv.Value))
                {
                    Title = "Validation Failed",
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Instance = context.HttpContext.Request.Path
                });
                context.ExceptionHandled = true;
                break;

            default:
                _logger.LogError(context.Exception, "Unhandled exception in action filter");
                break;
        }
    }
}
