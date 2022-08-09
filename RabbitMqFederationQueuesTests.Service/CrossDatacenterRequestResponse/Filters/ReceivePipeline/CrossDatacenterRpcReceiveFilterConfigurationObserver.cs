using MassTransit;
using MassTransit.Configuration;

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
