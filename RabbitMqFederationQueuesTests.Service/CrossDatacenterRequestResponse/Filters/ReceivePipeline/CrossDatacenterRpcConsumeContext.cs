using MassTransit;
using MassTransit.Context;

namespace RabbitMqFederationQueuesTests.Service;

class CrossDatacenterRpcConsumeContext<T> : ConsumeContextProxy<T> where T : class
{
    private readonly ConsumeContext _context;

    public override Uri ResponseAddress { get; }
    public override Headers Headers { get; }

    public CrossDatacenterRpcConsumeContext(Uri responseAddressOverride, ConsumeContext<T> context, Headers headers)
        : base(context)
    {
        _context = context;
        ResponseAddress = responseAddressOverride;
        Headers = headers;
    }

    public override bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
    {
        if (_context.TryGetMessage(out ConsumeContext<T> context))
        {
            consumeContext = new MessageConsumeContext<T>(this, context.Message);
            return true;
        }
        else
        {
            consumeContext = null!;
            return false;
        }
    }
}
