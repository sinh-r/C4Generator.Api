namespace C4Generator.Application.Abstractions;

public interface IKafkaConsumer<T> where T : class
{
    Task StartAsync(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken);
    void Stop();
}
