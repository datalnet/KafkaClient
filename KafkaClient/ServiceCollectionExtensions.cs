using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace KafkaClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaClient<TService>(
        this IServiceCollection serviceCollection,
        Action<KafkaClientOptionsBuilder>? options = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped
        ) where TService : KafkaClientCoreService
        => AddKafkaClient<TService, TService>(serviceCollection, options, contextLifetime, optionsLifetime);

    public static IServiceCollection AddKafkaClient<TService, TServiceImplementation>(
        this IServiceCollection serviceCollection,
        Action<KafkaClientOptionsBuilder>? optionsAction = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TServiceImplementation : KafkaClientCoreService, TService
        => AddKafkaClient<TService, TServiceImplementation>(
            serviceCollection,
            optionsAction == null
                ? null
                : (_, b) => optionsAction(b), contextLifetime, optionsLifetime);

    public static IServiceCollection AddKafkaClient<TService, TServiceImplementation>(
        this IServiceCollection serviceCollection,
        Action<IServiceProvider, KafkaClientOptionsBuilder>? optionsAction,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TServiceImplementation : KafkaClientCoreService, TService
    {
        if (contextLifetime == ServiceLifetime.Singleton)
        {
            optionsLifetime = ServiceLifetime.Singleton;
        }

        if (optionsAction != null)
        {
            CheckContextConstructors<TServiceImplementation>();
        }

        AddCoreServices<TServiceImplementation>(serviceCollection, optionsAction, optionsLifetime);

        serviceCollection.TryAdd(new ServiceDescriptor(typeof(TService), typeof(TServiceImplementation), contextLifetime));

        if (typeof(TService) != typeof(TServiceImplementation))
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(TServiceImplementation),
                    p => (TServiceImplementation)p.GetService<TService>()!,
                    contextLifetime));
        }

        return serviceCollection;
    }

    private static void AddCoreServices<TServiceImplementation>(
        IServiceCollection serviceCollection,
        Action<IServiceProvider, KafkaClientOptionsBuilder> optionsAction,
        ServiceLifetime optionsLifetime)
        where TServiceImplementation : KafkaClientCoreService
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(KafkaClientOptions<TServiceImplementation>),
                p => CreateDbContextOptions<TServiceImplementation>(p, optionsAction),
                optionsLifetime));

        serviceCollection.Add(
            new ServiceDescriptor(
                typeof(KafkaClientOptions),
                p => p.GetRequiredService<KafkaClientOptions<TServiceImplementation>>(),
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

    private static KafkaClientOptions<TServiceImplementation> CreateDbContextOptions<TServiceImplementation>(
        IServiceProvider applicationServiceProvider,
        Action<IServiceProvider, KafkaClientOptionsBuilder>? optionsAction)
        where TServiceImplementation : KafkaClientCoreService
    {
         var builder = new KafkaClientOptionsBuilder<TServiceImplementation>(
              new KafkaClientOptions<TServiceImplementation>(new Dictionary<Type, IKafkaClientOptionsExtension>()));

        //builder.UseApplicationServiceProvider(applicationServiceProvider);

        optionsAction?.Invoke(applicationServiceProvider, builder);

        return builder.Options;
    }

}
