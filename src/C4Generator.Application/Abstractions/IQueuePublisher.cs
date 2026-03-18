namespace C4Generator.Application.Abstractions;

public interface IQueuePublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
