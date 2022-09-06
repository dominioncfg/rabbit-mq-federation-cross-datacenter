using GreenPipes;
using MassTransit;
using MassTransit.Serialization;
using RabbitMqFederationQueuesTests.Contracts;

namespace RabbitMqFederationQueuesTests.Service;

public class CrossDatacenterRpcReceiveFilter<T> :
    IFilter<ConsumeContext<T>>
    where T : class
{
    private readonly RabbitMqConfiguration _appConfig;


    public CrossDatacenterRpcReceiveFilter(RabbitMqConfiguration appConfig)
    {
        _appConfig = appConfig;
    }


    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope(nameof(CrossDatacenterRpcReceiveFilter<T>));
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        if (!IsCrossDatacenterRequest(context))
        {
            await next.Send(context);
            return;
        }
        var newResponseAddress = GetResponseAddress(context);
        var newHeaders = StoreCrossDatacenterRequiredHeaders(context);

        var modifiedContext = new CrossDatacenterRpcConsumeContext<T>(newResponseAddress, context, newHeaders);
        await next.Send(modifiedContext);
    }

    private static bool IsCrossDatacenterRequest(ConsumeContext<T> context)
    {
        return (context.Message is ICrossDatacenterRpcRequest);
    }

    private Uri GetResponseAddress(ConsumeContext context)
    {
        if (!context.Headers.TryGetHeader(ConfigurationConstants.Messaging.Headers.DatacenterId,
            out var datacenterId) || !(datacenterId is string))
            throw new InvalidOperationException($"Cross datacenter request Error. Header {ConfigurationConstants.Messaging.Headers.DatacenterId} dont exist.");

        var respondToFederatedExchangeName = _appConfig.IsCurrentDatacenter($"{datacenterId}") ?
                    ConfigurationConstants.Messaging.GetInboundExchangeName(_appConfig.DatacenterId) :
                    ConfigurationConstants.Messaging.GetOutboundExchangeName(_appConfig.MainDatacenter);
        return new Uri($"exchange:{respondToFederatedExchangeName}");
    }

    private static JsonEnvelopeHeaders StoreCrossDatacenterRequiredHeaders(ConsumeContext<T> context)
    {
        var allHeaders = context.Headers.ToDictionary(x => x.Key, x => x.Value);

        allHeaders.Add(ConfigurationConstants.Messaging.Headers.RpcLocalQueue, context.ResponseAddress!);
        allHeaders.Add(ConfigurationConstants.Messaging.Headers.RpcRequestId, context.RequestId.ToString()!);

        return new JsonEnvelopeHeaders(allHeaders);
    }
}
