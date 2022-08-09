namespace RabbitMqFederationQueuesTests.Service;

public static class ConfigurationConstants
{
    public static class Messaging
    {
        private const string InboxQueuesSuffix = "inbound";
        private const string OutboundQueuesSuffix = "outbound";
        public const string BroadCastExchangeName = "broadcast";
        public static class Headers
        {
            public const string ResponseFederatedQueue = "R_FederatedQueue";
            public const string RpcLocalQueue = "ResponseQueue";
            public const string RpcRequestId = "R_Id";
        }

        public static string GetOutboundExchangeName(string datacenterId)
        {
            return $"{datacenterId}-{OutboundQueuesSuffix}";
        }

        public static string GetInboundExchangeName(string datacenterId)
        {
            return $"{datacenterId}-{InboxQueuesSuffix}";
        }
    }
}
