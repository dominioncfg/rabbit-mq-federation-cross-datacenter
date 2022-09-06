using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.PipeConfigurators;

namespace RabbitMqFederationQueuesTests.Service;

public class CrossDatacenterRpcReceiveFilterConfigurationObserver :
    ConfigurationObserver,
    IMessageConfigurationObserver
{
    public CrossDatacenterRpcReceiveFilterConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator)
        : base(receiveEndpointConfigurator)
    {
        Connect(this);
    }

    public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
        where TMessage : class
    {
        configurator.AddPipeSpecification(new CrossDatacenterRpcReceiveFilterPipeSpecification<TMessage>());
    }
}
