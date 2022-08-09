namespace RabbitMqFederationQueuesTests.Service;

public record RabbitMqConfiguration
{
    public const string SectionName = "RabbitQm";
    public string Host { get; init; } = string.Empty;
    public ushort Port { get; init; }
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string DatacenterId { get; init; } = string.Empty;
    public bool SendSampleMessage { get; init; } = true;
    public string MainDatacenter { get; init; } = string.Empty;

    public bool IsTheMainDatacenter()
    {
        return !string.IsNullOrEmpty(MainDatacenter) &&
               !string.IsNullOrEmpty(DatacenterId) &&
               MainDatacenter == DatacenterId;
    }

    public string[] AllDatacentersIds { get; init; } = Array.Empty<string>();
}
