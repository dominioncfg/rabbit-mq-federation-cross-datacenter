using GreenPipes;
using MassTransit;

namespace RabbitMqFederationQueuesTests.Service;

public class CrossDatacenterRpcReceiveFilterPipeSpecification<T> :
    IPipeSpecification<ConsumeContext<T>>
    where T : class
{
    private readonly RabbitMqConfiguration _rabbitMqConfiguration;

    public CrossDatacenterRpcReceiveFilterPipeSpecification(RabbitMqConfiguration rabbitMqConfiguration)
    {
        this._rabbitMqConfiguration = rabbitMqConfiguration;
    }

    public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
    {
        builder.AddFilter((new CrossDatacenterRpcReceiveFilter<T>(_rabbitMqConfiguration)));
    }

    public IEnumerable<ValidationResult> Validate()
    {
        yield break;
    }
}
