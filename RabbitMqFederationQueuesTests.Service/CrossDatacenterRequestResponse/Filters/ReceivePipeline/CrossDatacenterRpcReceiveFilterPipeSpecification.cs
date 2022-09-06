using GreenPipes;
using MassTransit;

namespace RabbitMqFederationQueuesTests.Service;

public class CrossDatacenterRpcReceiveFilterPipeSpecification<T> :
    IPipeSpecification<ConsumeContext<T>>
    where T : class
{
    public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
    {
        builder.AddFilter(new CrossDatacenterRpcReceiveFilter<T>());
    }

    public IEnumerable<ValidationResult> Validate()
    {
        yield break;
    }
}
