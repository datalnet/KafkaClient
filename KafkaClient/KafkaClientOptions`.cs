using System.Collections.Immutable;

namespace KafkaClient;

public class KafkaClientOptions<TContext> : KafkaClientOptions where TContext : KafkaClientCoreService
{
    public KafkaClientOptions()
    {
    }

    public KafkaClientOptions(IReadOnlyDictionary<Type, IKafkaClientOptionsExtension> extensions) : base(extensions)
    {
    }

    private KafkaClientOptions(
       ImmutableSortedDictionary<Type, (IKafkaClientOptionsExtension Extension, int Ordinal)> extensions)
       : base(extensions)
    {
    }

    public override KafkaClientOptions WithExtension<TExtension>(TExtension extension)
    {
        var type1 = typeof(TContext);

        var type = extension.GetType();

        var ordinal = ExtensionsMap.Count;

        if (ExtensionsMap.TryGetValue(type, out var existingValue))
        {
            ordinal = existingValue.Ordinal;
        }

        return new KafkaClientOptions<TContext>(ExtensionsMap.SetItem(type, (extension, ordinal)));
    }

    public override Type ContextType
       => typeof(TContext);
}
