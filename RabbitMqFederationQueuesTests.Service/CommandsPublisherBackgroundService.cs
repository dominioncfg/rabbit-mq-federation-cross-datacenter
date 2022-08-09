using MassTransit;
using Microsoft.Extensions.Options;
using RabbitMqFederationQueuesTests.Contracts;

namespace RabbitMqFederationQueuesTests.Service;

public class CommandsPublisherBackgroundService : BackgroundService
{
    private readonly IBus _bus;
    private readonly ILogger<CommandsPublisherBackgroundService> _logger;
    private readonly RabbitMqConfiguration _appConfig;

    public CommandsPublisherBackgroundService(IBus bus, ILogger<CommandsPublisherBackgroundService> logger, IOptions<RabbitMqConfiguration> appConfig)
    {

        _bus = bus;
        _logger = logger;
        _appConfig = appConfig.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await WaitUntilRabbitHasStarted();

        if (_appConfig.SendSampleMessage)
        {
            await SendMessagesUnlessStopped(cancellationToken);
        }
    }

    private static async Task WaitUntilRabbitHasStarted()
    {
        await Task.Delay(5000);
    }

    private async Task SendMessagesUnlessStopped(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await SendMessagesAtGivenCadence(cancellationToken);
            await Task.Delay(1000, cancellationToken);
        }
    }

    protected async Task SendMessagesAtGivenCadence(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending the 2 messages...");

        var random = new Random();
        var entrophy = random.Next();

        await SendCreateUserCommand(entrophy, cancellationToken);
        await SendCreateAccountCommand(entrophy, cancellationToken);
    }

    private async Task SendCreateUserCommand(int entrophy, CancellationToken cancellationToken)
    {
        var message = new CreateAccountCommand
        {
            Id = Guid.NewGuid(),
            Name = $"Account {entrophy}",
            Users = entrophy,
        };

        var client = _bus.CreateRequestClient<CreateUserCommand>();
        _logger.LogInformation("Sended message of type {MessageType} with content{Message}", message.GetType().Name, message);
        var response = await client.GetResponse<CreateUserCommandResponse>(message, cancellationToken, RequestTimeout.After(m: 5));
        _logger.LogInformation("Received response from server of type {MessageType} with content {Message}", response.Message.GetType().Name, response.Message);
    }

    private async Task SendCreateAccountCommand(int entrophy, CancellationToken cancellationToken)
    {
        var message = new CreateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = $"FN {entrophy}",
            LastName = $"LN {entrophy} from Datacenter {_appConfig.DatacenterId}",
        };

        var client = _bus.CreateRequestClient<CreateAccountCommand>();
        _logger.LogInformation("Sended message of type {MessageType} with content{Message}", message.GetType().Name, message);
        var response = await client.GetResponse<CreateAccountCommandResponse>(message, cancellationToken, RequestTimeout.After(m: 5));
        _logger.LogInformation("Received response from server of type {MessageType} with content {Message}", response.Message.GetType().Name, response.Message);
    }
}

