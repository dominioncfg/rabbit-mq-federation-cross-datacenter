using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Newtonsoft.Json;
using RabbitMqFederationQueuesTests.Contracts;

namespace RabbitMqFederationQueuesTests.Service;

public class CrossDatacenterRpcResponseConsumer : IConsumer<CrossDatacenterRpcResponse>
{
    public async Task Consume(ConsumeContext<CrossDatacenterRpcResponse> context)
    {
        var type = Type.GetType(context.Message.MessageType)!;
        var deserializedObject = JsonConvert.DeserializeObject(context.Message.MessagePayload, type)!;
        var destinationAddress = context.Headers.Get<string>(ConfigurationConstants.Messaging.Headers.RpcLocalQueue)!;
        var requestId = context.Headers.Get<string>(ConfigurationConstants.Messaging.Headers.RpcRequestId)!;

        var pipe = Pipe.New<SendContext>(x =>
        {
            x.UseExecute(y => AddHeadersToRequest(y, requestId, destinationAddress));
        });

        await context.Send(new Uri(destinationAddress), deserializedObject, deserializedObject.GetType(), pipe);
    }

    private static void AddHeadersToRequest(SendContext x, string requestId, string destinationAddress)
    {
        x.Headers.Set(ConfigurationConstants.Messaging.Headers.RpcRequestId, x.RequestId.ToString());
        x.RequestId = Guid.Parse(requestId);
        x.DestinationAddress = new Uri(destinationAddress);
        x.ResponseAddress = new Uri(destinationAddress);
    }
}

public class CrossDatacenterRpcResponseConsumerDefinition : ConsumerDefinition<CrossDatacenterRpcResponseConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CrossDatacenterRpcResponseConsumer> consumerConfigurator)
    {
        endpointConfigurator.ConfigureConsumeTopology = false;
    }
}
