using CashFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.DTOs
{
    public sealed record TransactionDto(
   string? Id,
   string Type,
   decimal Amount,
   DateOnly Date,
   string Description
    )
    {
        public TransactionDto() : this(null, string.Empty, 0, default, string.Empty) { }
    }
}
