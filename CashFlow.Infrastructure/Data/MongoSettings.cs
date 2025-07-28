using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Infrastructure.Data
{
    public sealed record MongoSettings
    {
        public required string ConnectionString { get; init; }
        public required string DatabaseName { get; init; }
    }
}
