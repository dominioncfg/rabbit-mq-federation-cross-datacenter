using MassTransit;
using RabbitMqFederationQueuesTests.Contracts;
using System.Reflection;


namespace RabbitMqFederationQueuesTests.Service;

public static class ConfigurationExtensions
{
    public static void AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqConfig = new RabbitMqConfiguration();
        configuration.GetSection(RabbitMqConfiguration.SectionName).Bind(rabbitMqConfig);
        services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.SectionName));

        services.AddMassTransit(busConfigurator =>
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.AddConsumers(entryAssembly);

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {            
                cfg.UseCrossDatacenterRpcFilters(context, rabbitMqConfig);

                cfg.Host(rabbitMqConfig.Host, rabbitMqConfig.Port, "/", h =>
                 {
                     h.Username(rabbitMqConfig.User);
                     h.Password(rabbitMqConfig.Password);
                 });

                var queueName = ConfigurationConstants.Messaging.GetInboundExchangeName(rabbitMqConfig.DatacenterId);
                cfg.ReceiveEndpoint(queueName, e =>
                {
                    e.ConfigureConsumer<CreateUserCommandConsumer>(context);
                    e.ConfigureConsumer<CreateAccountCommandConsumer>(context);
                    e.ConfigureCrossDatacenterRpcConsumer(context);
                });
            });

            ConfigureSendCommands();
        });
    }

    private static void ConfigureSendCommands()
    {
        var exchangeName = ConfigurationConstants.Messaging.BroadCastExchangeName;
        EndpointConvention.Map<CreateUserCommand>(new Uri($"exchange:{exchangeName}"));
        EndpointConvention.Map<CreateAccountCommand>(new Uri($"exchange:{exchangeName}"));
    }
}
