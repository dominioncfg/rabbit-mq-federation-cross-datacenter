using Newtonsoft.Json;

namespace RabbitMqFederationQueuesTests.Contracts;

public interface ICrossDatacenterRpcRequest { }

public class CrossDatacenterRpcResponse
{
    public string MessageType { get; set; } = string.Empty;

    public string MessagePayload { get; set; } = string.Empty;

    public static CrossDatacenterRpcResponse FromObject(object o)
    {
        return new CrossDatacenterRpcResponse()
        {
            MessageType = o.GetType().AssemblyQualifiedName!,
            MessagePayload = JsonConvert.SerializeObject(o),
        };
    }
}
