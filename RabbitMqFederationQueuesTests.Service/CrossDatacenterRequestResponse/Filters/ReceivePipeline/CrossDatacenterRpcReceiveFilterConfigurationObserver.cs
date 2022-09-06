using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.PipeConfigurators;

namespace RabbitMqFederationQueuesTests.Service;

public class CrossDatacenterRpcReceiveFilterConfigurationObserver :
    ConfigurationObserver,
    IMessageConfigurationObserver
{
    private readonly RabbitMqConfiguration _rabbitMqConfiguration;

    public CrossDatacenterRpcReceiveFilterConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator,
        RabbitMqConfiguration rabbitMqConfiguration)
        : base(receiveEndpointConfigurator)
    {
        Connect(this);
        this._rabbitMqConfiguration = rabbitMqConfiguration;
    }

    public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
        where TMessage : class
    {
        configurator.AddPipeSpecification(new CrossDatacenterRpcReceiveFilterPipeSpecification<TMessage>(_rabbitMqConfiguration));
    }
}
