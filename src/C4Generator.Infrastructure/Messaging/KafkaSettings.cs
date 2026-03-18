namespace C4Generator.Infrastructure.Messaging;

public sealed class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string GroupId { get; set; } = "c4generator-group";
    public string Topic { get; set; } = "c4generator.architecture.generation";

    // mTLS (Aiven / SSL)
    public string SecurityProtocol { get; set; } = "ssl";
    public string? SslCaLocation { get; set; }
    public string? SslCertificateLocation { get; set; }
    public string? SslKeyLocation { get; set; }
    public string? SslKeyPassword { get; set; }
}
