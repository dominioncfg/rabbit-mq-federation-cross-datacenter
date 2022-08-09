using System.Text.Json;

namespace RabbitMqFederationQueuesTests.Contracts;

public record CreateAccountCommand: ICrossDatacenterRpcRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Users { get; init; }

    public override string ToString() => JsonSerializer.Serialize(this);
}

public record CreateAccountCommandResponse
{
    public bool Success { get; init; }

    public override string ToString() => JsonSerializer.Serialize(this);
}
