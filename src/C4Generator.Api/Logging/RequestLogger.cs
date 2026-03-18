namespace C4Generator.Api.Logging;

public sealed class RequestLogger
{
    private readonly ILogger<RequestLogger> _logger;

    public RequestLogger(ILogger<RequestLogger> logger)
    {
        _logger = logger;
    }

    public void LogRequest(HttpContext context)
    {
        _logger.LogInformation(
            "Inbound {Method} {Scheme}://{Host}{Path}{QueryString} from {RemoteIp}",
            context.Request.Method,
            context.Request.Scheme,
            context.Request.Host,
            context.Request.Path,
            context.Request.QueryString,
            context.Connection.RemoteIpAddress);
    }

    public void LogResponse(HttpContext context, long elapsedMs)
    {
        _logger.LogInformation(
            "Outbound {StatusCode} for {Method} {Path} in {Elapsed}ms",
            context.Response.StatusCode,
            context.Request.Method,
            context.Request.Path,
            elapsedMs);
    }
}
