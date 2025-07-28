using CashFlow.Application.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;

namespace CashFlow.Infrastructure.MessageBus
{
    public sealed class RabbitMqMessageBus : IMessageBus, IDisposable
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly RabbitMqSettings _settings;

        public RabbitMqMessageBus(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_settings.Exchange, ExchangeType.Topic, durable: true);
        }

        public Task PublishAsync<T>(string routingKey, T message, CancellationToken ct = default)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var props = _channel.CreateBasicProperties();
            props.Persistent = true;

            _channel.BasicPublish(exchange: _settings.Exchange,
                                  routingKey: routingKey,
                                  basicProperties: props,
                                  body: body);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
