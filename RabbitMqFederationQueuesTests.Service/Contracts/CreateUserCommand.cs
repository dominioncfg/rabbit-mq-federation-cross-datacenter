using System.Text.Json;

namespace RabbitMqFederationQueuesTests.Contracts;

public record CreateUserCommand: ICrossDatacenterRpcRequest
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;

    public override string ToString() => JsonSerializer.Serialize(this);
}

public record CreateUserCommandResponse
{
    public bool Success { get; init; }

    public override string ToString() => JsonSerializer.Serialize(this);
}
