using C4Generator.Application.Abstractions;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace C4Generator.Infrastructure.Messaging;

internal sealed class KafkaConsumer<T> : IKafkaConsumer<T>, IDisposable where T : class
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topic;
    private readonly ILogger<KafkaConsumer<T>> _logger;
    private bool _running;

    public KafkaConsumer(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaConsumer<T>> logger)
    {
        _logger = logger;
        var settings = kafkaSettings.Value;
        _topic = settings.Topic;

        var config = new ConsumerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            GroupId = settings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SecurityProtocol = ParseSecurityProtocol(settings.SecurityProtocol)
        };

        if (!string.IsNullOrEmpty(settings.SslCaLocation))
            config.SslCaLocation = settings.SslCaLocation;
        if (!string.IsNullOrEmpty(settings.SslCertificateLocation))
            config.SslCertificateLocation = settings.SslCertificateLocation;
        if (!string.IsNullOrEmpty(settings.SslKeyLocation))
            config.SslKeyLocation = settings.SslKeyLocation;
        if (!string.IsNullOrEmpty(settings.SslKeyPassword))
            config.SslKeyPassword = settings.SslKeyPassword;

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }

    public async Task StartAsync(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);
        _running = true;
        _logger.LogInformation("KafkaConsumer<{Type}> subscribed to topic '{Topic}'", typeof(T).Name, _topic);

        while (_running && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(TimeSpan.FromSeconds(1));
                if (result is null) continue;

                _logger.LogDebug("Received message on partition {Partition} offset {Offset}", result.Partition.Value, result.Offset.Value);

                var message = JsonSerializer.Deserialize<T>(result.Message.Value);
                if (message is null)
                {
                    _logger.LogWarning("Could not deserialize Kafka message: {Value}", result.Message.Value);
                    _consumer.Commit(result);
                    continue;
                }

                await handler(message, cancellationToken);
                _consumer.Commit(result);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error processing Kafka message");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    public void Stop()
    {
        _running = false;
        _consumer.Unsubscribe();
    }

    public void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
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
