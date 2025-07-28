using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.DTOs
{
    public sealed record DailyBalanceDto(
    DateOnly Date,
    decimal TotalCredits,
    decimal TotalDebits,
    decimal Balance
);
}
