using MassTransit;
using Microsoft.Extensions.Options;
using RabbitMqFederationQueuesTests.Contracts;

namespace RabbitMqFederationQueuesTests.Service;

public class CreateAccountCommandConsumer : IConsumer<CreateAccountCommand>
{
    readonly ILogger<CreateAccountCommandConsumer> _logger;
    private readonly IObjectsRepository _objectsRepository;
    private readonly RabbitMqConfiguration _config;

    public CreateAccountCommandConsumer(ILogger<CreateAccountCommandConsumer> logger, IObjectsRepository objectsRepository, IOptions<RabbitMqConfiguration> config)
    {
        _logger = logger;
        _objectsRepository = objectsRepository;
        _config = config.Value;
    }

    public async Task Consume(ConsumeContext<CreateAccountCommand> context)
    {
        if(!_config.IsTheMainDatacenter())
            return;

        _logger.LogInformation("Received Text: {Text}", context.Message);
        _objectsRepository.Add(context.Message);
        var random = new Random();
        var delayFor = random.Next(200, 300);
        await Task.Delay(delayFor);
        await context.RespondAsync(CrossDatacenterRpcResponse.FromObject(new CreateAccountCommandResponse() { Success = true }));
    }
}

public class CreateAccountCommandConsumerDefinition : ConsumerDefinition<CreateAccountCommandConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateAccountCommandConsumer> consumerConfigurator)
    {
        endpointConfigurator.ConfigureConsumeTopology = false;
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
    }
}

