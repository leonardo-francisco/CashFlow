using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.Contracts
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(string routingKey, T message, CancellationToken ct = default);
    }
}
