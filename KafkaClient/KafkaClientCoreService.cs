using Microsoft.Extensions.Options;

namespace KafkaClient;

public abstract class KafkaClientCoreService
{
    private readonly KafkaClientOptions _kafkaClientOptions;

    private KafkaClientModel _kafkaClientModel = default!;

    protected KafkaClientCoreService()
       : this(new KafkaClientOptions<KafkaClientCoreService>())
    {
    }

    public KafkaClientCoreService(KafkaClientOptions kafkaClientOptions)
    {
        if (!kafkaClientOptions.ContextType.IsAssignableFrom(GetType()))
        {
            throw new InvalidOperationException("NonGenericOptions");
        }

        _kafkaClientOptions = kafkaClientOptions;

        SetModel();
    }

    private void SetModel()
    {

        var optionsBuilder = new KafkaClientOptionsBuilder(_kafkaClientOptions);

        var options = optionsBuilder.Options;

        //var option = _kafkaClientOptions.GetExtension<CoreOptionsExtension>();

        //if (option != null)
        //{
        //    _kafkaClientModel = option.KafkaClientModel;
        //}
        //else
        //{
        //    throw new NullReferenceException("Model not configured. Add one in AddIndexedDbDatabase method");
        //}
    }
}
