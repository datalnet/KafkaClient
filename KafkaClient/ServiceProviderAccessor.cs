namespace KafkaClient;

public class ServiceProviderAccessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceProviderAccessor" /> class.
    /// </summary>
    /// <param name="rootServiceProvider">The injected service provider.</param>
    public ServiceProviderAccessor(IServiceProvider rootServiceProvider)
    {
        RootServiceProvider = rootServiceProvider;
    }

    /// <summary>
    ///     The injected service provider.
    /// </summary>
    public virtual IServiceProvider RootServiceProvider { get; }
}
