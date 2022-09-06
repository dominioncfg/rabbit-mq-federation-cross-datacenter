using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Options;
using RabbitMqFederationQueuesTests.Contracts;


namespace RabbitMqFederationQueuesTests.Service;

public class CrossDatacenterRpcSendFilter<T> :
    IFilter<SendContext<T>>
    where T : class
{
    private readonly RabbitMqConfiguration _appConfig;


    public CrossDatacenterRpcSendFilter(IOptions<RabbitMqConfiguration> appConfig)
    {
        _appConfig = appConfig.Value;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope(nameof(CrossDatacenterRpcSendFilter<T>));
    }

    public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        if (!IsCrossDatacenterRequest(context))
            return Task.CompletedTask;
        AddCrossDatacenterRequestRequiredHeaders(context);
        return Task.CompletedTask;
    }

    private static bool IsCrossDatacenterRequest(SendContext<T> context)
    {
        return (context.Message is ICrossDatacenterRpcRequest);
    }

    private void AddCrossDatacenterRequestRequiredHeaders(SendContext<T> context)
    {
        var respondToFederatedExchangeName = _appConfig.IsTheMainDatacenter() ?
                    ConfigurationConstants.Messaging.GetInboundExchangeName(_appConfig.DatacenterId) :
                    ConfigurationConstants.Messaging.GetOutboundExchangeName(_appConfig.MainDatacenter);
        context.Headers.Set(ConfigurationConstants.Messaging.Headers.ResponseFederatedQueue, $"exchange:{respondToFederatedExchangeName}");
    }
}