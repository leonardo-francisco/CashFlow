using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.Contracts
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default);
        Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
        Task RemoveAsync(string key, CancellationToken ct = default);
    }
}
