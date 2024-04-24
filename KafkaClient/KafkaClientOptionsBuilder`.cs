namespace KafkaClient;

public class KafkaClientOptionsBuilder<TContext> : KafkaClientOptionsBuilder where TContext : KafkaClientCoreService
{
    public KafkaClientOptionsBuilder()
       : this(new KafkaClientOptions<TContext>())
    {
    }

    public KafkaClientOptionsBuilder(KafkaClientOptions<TContext> options) : base(options)
    {
    }

    public new virtual KafkaClientOptions<TContext> Options => (KafkaClientOptions<TContext>)base.Options;

    public new virtual KafkaClientOptionsBuilder<TContext> UseModel(KafkaClientModel model) 
        => (KafkaClientOptionsBuilder<TContext>)base.UseModel(model);

    //public new virtual KafkaClientOptionsBuilder<TContext> UseApplicationServiceProvider(IServiceProvider? serviceProvider)
    //    => (KafkaClientOptionsBuilder<TContext>)base.UseApplicationServiceProvider(serviceProvider);
}
