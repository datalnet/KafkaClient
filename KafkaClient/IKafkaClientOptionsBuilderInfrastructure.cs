namespace KafkaClient;

public interface IKafkaClientOptionsBuilderInfrastructure
{
    void AddOrUpdateExtension<TExtension>(TExtension extension) where TExtension : class, IKafkaClientOptionsExtension;
}
