using System.Collections.Immutable;

namespace KafkaClient;

public abstract class KafkaClientOptions : IKafkaClientOptions
{
    private readonly ImmutableSortedDictionary<Type, (IKafkaClientOptionsExtension Extension, int Ordinal)> _extensionsMap;

    protected KafkaClientOptions()
    {
        _extensionsMap = ImmutableSortedDictionary.Create<Type, (IKafkaClientOptionsExtension, int)>(TypeFullNameComparer.Instance);
    }

    protected KafkaClientOptions(
        IReadOnlyDictionary<Type, IKafkaClientOptionsExtension> extensions)
    {
        _extensionsMap = ImmutableSortedDictionary.Create<Type, (IKafkaClientOptionsExtension, int)>(TypeFullNameComparer.Instance)
            .AddRange(extensions.Select((p, i) => new KeyValuePair<Type, (IKafkaClientOptionsExtension, int)>(p.Key, (p.Value, i))));
    }

    protected KafkaClientOptions(
       ImmutableSortedDictionary<Type, (IKafkaClientOptionsExtension Extension, int Ordinal)> extensions)
    {
        _extensionsMap = extensions;
    }

    public virtual IEnumerable<IKafkaClientOptionsExtension> Extensions
       => _extensionsMap.Values.OrderBy(v => v.Ordinal).Select(v => v.Extension);

    protected virtual ImmutableSortedDictionary<Type, (IKafkaClientOptionsExtension Extension, int Ordinal)> ExtensionsMap
      => _extensionsMap;

    public virtual TExtension? FindExtension<TExtension>()
        where TExtension : class, IKafkaClientOptionsExtension
        => _extensionsMap.TryGetValue(typeof(TExtension), out var value) ? (TExtension)value.Extension : null;

    public virtual TExtension GetExtension<TExtension>() where TExtension : class, IKafkaClientOptionsExtension
    {
        var extension = FindExtension<TExtension>();
        if (extension == null)
        {
            throw new InvalidOperationException($"OptionsExtensionNotFound {typeof(TExtension)}");
        }

        return extension;
    }

    public abstract KafkaClientOptions WithExtension<TExtension>(TExtension extension)
        where TExtension : class, IKafkaClientOptionsExtension;

    public abstract Type ContextType { get; }
}
