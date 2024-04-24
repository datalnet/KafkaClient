namespace KafkaClient;

public class KafkaClientOptionsBuilder : IKafkaClientOptionsBuilderInfrastructure
{
    private KafkaClientOptions _options;

    public KafkaClientOptionsBuilder(KafkaClientOptions options)
    {
        _options = options;
    }

    public virtual KafkaClientOptions Options => _options;

    public virtual KafkaClientOptionsBuilder UseModel(KafkaClientModel KafkaClientModel) => WithOption(e => e.WithModel(KafkaClientModel));

    void IKafkaClientOptionsBuilderInfrastructure.AddOrUpdateExtension<TExtension>(TExtension extension)
    {
        _options = _options.WithExtension(extension);
    }

    private KafkaClientOptionsBuilder WithOption(Func<CoreOptionsExtension, CoreOptionsExtension> withFunc)
    {
        ((IKafkaClientOptionsBuilderInfrastructure)this).AddOrUpdateExtension(withFunc(Options.FindExtension<CoreOptionsExtension>() ?? new CoreOptionsExtension()));

        return this;
    }

    public virtual KafkaClientOptionsBuilder UseApplicationServiceProvider(IServiceProvider serviceProvider)
        => WithOption(e => e.WithApplicationServiceProvider(serviceProvider));
}
