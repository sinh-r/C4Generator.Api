using C4Generator.Application.Abstractions;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace C4Generator.Infrastructure.Messaging;

internal sealed class KafkaQueuePublisher : IQueuePublisher, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaQueuePublisher(IOptions<KafkaSettings> kafkaSettings)
    {
        var settings = kafkaSettings.Value;
        _topic = settings.Topic;

        var config = new ProducerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            SecurityProtocol = ParseSecurityProtocol(settings.SecurityProtocol),
            SslCaLocation = settings.SslCaLocation,
            SslCertificateLocation = settings.SslCertificateLocation,
            SslKeyLocation = settings.SslKeyLocation,
            SslKeyPassword = settings.SslKeyPassword
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        var json = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json }, cancellationToken);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
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
