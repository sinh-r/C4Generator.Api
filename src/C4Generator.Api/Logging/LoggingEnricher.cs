using Serilog.Core;
using Serilog.Events;

namespace C4Generator.Api.Logging;

public sealed class LoggingEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggingEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null) return;

        if (context.Items.TryGetValue("X-Correlation-Id", out var correlationId))
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("CorrelationId", correlationId?.ToString() ?? string.Empty));
        }

        var userId = context.User?.FindFirst("sub")?.Value
                  ?? context.User?.Identity?.Name
                  ?? "anonymous";

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("UserId", userId));
    }
}
