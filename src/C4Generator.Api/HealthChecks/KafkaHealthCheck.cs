using C4Generator.Infrastructure.Messaging;
using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace C4Generator.Api.HealthChecks;

public sealed class KafkaHealthCheck : IHealthCheck
{
    private readonly KafkaSettings _settings;

    public KafkaHealthCheck(IOptions<KafkaSettings> kafkaSettings)
    {
        _settings = kafkaSettings.Value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                SecurityProtocol = ParseSecurityProtocol(_settings.SecurityProtocol),
                SslCaLocation = _settings.SslCaLocation,
                SslCertificateLocation = _settings.SslCertificateLocation,
                SslKeyLocation = _settings.SslKeyLocation,
                SslKeyPassword = _settings.SslKeyPassword
            };

            using var adminClient = new AdminClientBuilder(config).Build();
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));

            return Task.FromResult(
                HealthCheckResult.Healthy($"Kafka is reachable. Brokers: {metadata.Brokers.Count}"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy("Kafka is unreachable.", ex));
        }
    }

    private static SecurityProtocol ParseSecurityProtocol(string value) =>
        value.ToLowerInvariant() switch
        {
            "ssl"            => SecurityProtocol.Ssl,
            "sasl_ssl"       => SecurityProtocol.SaslSsl,
            "sasl_plaintext" => SecurityProtocol.SaslPlaintext,
            _                => SecurityProtocol.Plaintext
        };
}
