using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Ora.Common.Commands;
using Ora.Common.CommandsHandler;
using Ora.Common.RabbitMq;
using RawRabbit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Common.Services
{
    public static class BusBuilderExtension
    {
        public static IBusClient RabbitMQ(this IHost host)
        {
            return (IBusClient)host.Services.GetService(typeof(IBusClient));
        }

        public static void SubscribeToCommand<TCommand>(this IBusClient bus, IHost host) where TCommand : ICommand
        {
            var handler = (ICommandHandler<TCommand>)host.Services
                .GetService(typeof(ICommandHandler<TCommand>));
            bus.WithCommandHandlerAsync(handler);
        }

        public static void SubscribeToEvent<TEvent>(this IBusClient bus, IHost host) where TEvent : IEvent
        {
            var handler = (IEventHandler<TEvent>)host.Services
                .GetService(typeof(IEventHandler<TEvent>));
            bus.WithEventHandlerAsync(handler);
        }
    }
}
