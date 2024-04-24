namespace KafkaClient;

public class CoreOptionsExtension : IKafkaClientOptionsExtension
{
    private IServiceProvider? _applicationServiceProvider;

    private KafkaClientModel _kafkaClientModel = default!;

    public CoreOptionsExtension()
    {
    }

    protected CoreOptionsExtension(CoreOptionsExtension copyFrom)
    {
        _applicationServiceProvider = copyFrom.ApplicationServiceProvider;
    }

    public virtual KafkaClientModel KafkaClientModel => _kafkaClientModel;

    protected virtual CoreOptionsExtension Clone() => new(this);

    public virtual IServiceProvider? ApplicationServiceProvider => _applicationServiceProvider;

    public virtual CoreOptionsExtension WithApplicationServiceProvider(IServiceProvider applicationServiceProvider)
    {
        var clone = Clone();

        clone._applicationServiceProvider = applicationServiceProvider;

        return clone;
    }

    public virtual CoreOptionsExtension WithModel(KafkaClientModel model)
    {
        var clone = Clone();

        clone._kafkaClientModel = model;

        return clone;
    }

}
