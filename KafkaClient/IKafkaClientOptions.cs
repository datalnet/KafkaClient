namespace KafkaClient;

public interface IKafkaClientOptions
{
    IEnumerable<IKafkaClientOptionsExtension> Extensions { get; }

    TExtension? FindExtension<TExtension>() where TExtension : class, IKafkaClientOptionsExtension;
}