using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Infrastructure.MessageBus
{
    public sealed class RabbitMqSettings
    {
        public required string HostName { get; init; }
        public int Port { get; init; } = 5672;
        public string UserName { get; init; } = "guest";
        public string Password { get; init; } = "guest";
        public string Exchange { get; init; } = "transactions.exchange";
    }
}
