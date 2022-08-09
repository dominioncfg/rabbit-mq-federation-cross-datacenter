using MassTransit;
using Microsoft.Extensions.Options;
using RabbitMqFederationQueuesTests.Contracts;

namespace RabbitMqFederationQueuesTests.Service; 

public class CreateUserCommandConsumer : IConsumer<CreateUserCommand>
{
    readonly ILogger<CreateUserCommandConsumer> _logger;
    private readonly IObjectsRepository _objectsRepository;
    private readonly RabbitMqConfiguration _config;

    public CreateUserCommandConsumer(ILogger<CreateUserCommandConsumer> logger, IObjectsRepository objectsRepository, IOptions<RabbitMqConfiguration> config)
    {
        _logger = logger;
        _objectsRepository = objectsRepository;
        _config = config.Value;
    }

    public async Task Consume(ConsumeContext<CreateUserCommand> context)
    {
        if (!_config.IsTheMainDatacenter())
            return;

        _logger.LogInformation("Received Text: {Text}", context.Message);
        _objectsRepository.Add(context.Message);
        var random = new Random();
        var delayFor = random.Next(200, 300);
        await Task.Delay(delayFor);
        await context.RespondAsync(CrossDatacenterRpcResponse.FromObject(new CreateUserCommandResponse() { Success = true }));
    }
}

public class CreateUserCommandConsumerDefinition : ConsumerDefinition<CreateUserCommandConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateUserCommandConsumer> consumerConfigurator)
    {
        endpointConfigurator.ConfigureConsumeTopology = false;
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
    }
}
