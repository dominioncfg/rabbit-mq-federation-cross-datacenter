﻿using MassTransit;
using MassTransit.RabbitMqTransport;

namespace RabbitMqFederationQueuesTests.Service;

public static class CrossDatacenterRpcExtensions
{
    public static void UseCrossDatacenterRpcFilters(this IRabbitMqBusFactoryConfigurator configurator, IBusRegistrationContext context, RabbitMqConfiguration rabbitConfig)
    {
        if (configurator == null)
            throw new ArgumentNullException(nameof(configurator));

        configurator.UseSendFilter(typeof(CrossDatacenterRpcSendFilter<>), context);

        _ = new CrossDatacenterRpcReceiveFilterConfigurationObserver(configurator, rabbitConfig);
    }

    public static void ConfigureCrossDatacenterRpcConsumer(this IRabbitMqReceiveEndpointConfigurator endpoint, IBusRegistrationContext context)
    {
        endpoint.ConfigureConsumer<CrossDatacenterRpcResponseConsumer>(context);
    }
}
