using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace KafkaClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaClient<TContext>(
        this IServiceCollection serviceCollection,
        Action<KafkaClientOptionsBuilder>? options = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped
        ) where TContext : KafkaClientCoreService
        => AddKafkaClient<TContext, TContext>(serviceCollection, options, contextLifetime, optionsLifetime);

    public static IServiceCollection AddKafkaClient<TContextService, TContextImplementation>(
        this IServiceCollection serviceCollection,
        Action<KafkaClientOptionsBuilder>? optionsAction = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TContextImplementation : KafkaClientCoreService, TContextService
        => AddKafkaClient<TContextService, TContextImplementation>(
            serviceCollection,
            optionsAction == null
                ? null
                : (_, b) => optionsAction(b), contextLifetime, optionsLifetime);

    public static IServiceCollection AddKafkaClient<TContextService, TContextImplementation>(
        this IServiceCollection serviceCollection,
        Action<IServiceProvider, KafkaClientOptionsBuilder>? optionsAction,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TContextImplementation : KafkaClientCoreService, TContextService
    {
        var type = typeof(TContextImplementation);

        if (contextLifetime == ServiceLifetime.Singleton)
        {
            optionsLifetime = ServiceLifetime.Singleton;
        }

        if (optionsAction != null)
        {
            CheckContextConstructors<TContextImplementation>();
        }

        AddCoreServices<TContextImplementation>(serviceCollection, optionsAction, optionsLifetime);

        //serviceCollection.TryAdd(new ServiceDescriptor(typeof(TContextService), typeof(TContextImplementation), contextLifetime));
        serviceCollection.AddScoped(typeof(TContextImplementation));

        //serviceCollection.AddKeyedScoped(typeof(TContextService), typeof(TContextImplementation), type);

        if (typeof(TContextService) != typeof(TContextImplementation))
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(TContextImplementation),
                    p => (TContextImplementation)p.GetService<TContextService>()!,
                    contextLifetime));
        }

        return serviceCollection;
    }

    private static void AddCoreServices<TContextImplementation>(
        IServiceCollection serviceCollection,
        Action<IServiceProvider, KafkaClientOptionsBuilder> optionsAction,
        ServiceLifetime optionsLifetime)
        where TContextImplementation : KafkaClientCoreService
    {
        var type = typeof(TContextImplementation);

        //serviceCollection.TryAddSingleton<ServiceProviderAccessor>();

        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(KafkaClientOptions<TContextImplementation>),
                p => CreateDbContextOptions<TContextImplementation>(p, optionsAction),
                optionsLifetime));

        serviceCollection.Add(
            new ServiceDescriptor(
                typeof(KafkaClientOptions),
                p => p.GetRequiredService<KafkaClientOptions<TContextImplementation>>(),
                optionsLifetime));
    }

    private static void CheckContextConstructors<TContext>()
        where TContext : KafkaClientCoreService
    {
        var declaredConstructors = typeof(TContext).GetTypeInfo().DeclaredConstructors.ToList();
        if (declaredConstructors.Count == 1 && declaredConstructors[0].GetParameters().Length == 0)
        {
            throw new ArgumentException($"DbContextMissingConstructor{typeof(TContext)}");
        }
    }

    private static KafkaClientOptions<TContext> CreateDbContextOptions<TContext>(
        IServiceProvider applicationServiceProvider,
        Action<IServiceProvider, KafkaClientOptionsBuilder>? optionsAction)
        where TContext : KafkaClientCoreService
    {

        var type = typeof(TContext);

         var builder = new KafkaClientOptionsBuilder<TContext>(
              new KafkaClientOptions<TContext>(new Dictionary<Type, IKafkaClientOptionsExtension>()));

        builder.UseApplicationServiceProvider(applicationServiceProvider);

        optionsAction?.Invoke(applicationServiceProvider, builder);

        return builder.Options;
    }

}
