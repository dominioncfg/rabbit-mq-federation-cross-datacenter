using MassTransit;
using MassTransit.Serialization;
using RabbitMqFederationQueuesTests.Contracts;

namespace RabbitMqFederationQueuesTests.Service;

public class CrossDatacenterRpcReceiveFilter<T> :
    IFilter<ConsumeContext<T>>
    where T : class
{
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

    private static Uri GetResponseAddress(ConsumeContext context)
    {
        if (!context.Headers.TryGetHeader(ConfigurationConstants.Messaging.Headers.ResponseFederatedQueue,
            out var federatedQueueAddress) || !(federatedQueueAddress is string))
            throw new InvalidOperationException("Cross datacenter request Error. Header {ConfigurationConstants.Messaging.Headers.ResponseFederatedQueue} dont exist.");
        
        return new Uri((string)federatedQueueAddress);
    }

    private static ReadOnlyDictionaryHeaders StoreCrossDatacenterRequiredHeaders(ConsumeContext<T> context)
    {
        var allHeaders = context.Headers.ToDictionary(x => x.Key, x => x.Value);

        allHeaders.Add(ConfigurationConstants.Messaging.Headers.RpcLocalQueue, context.ResponseAddress!.ToString());
        allHeaders.Add(ConfigurationConstants.Messaging.Headers.RpcRequestId, context.RequestId!);

        return new ReadOnlyDictionaryHeaders(context.SerializerContext, allHeaders);
    }
}
