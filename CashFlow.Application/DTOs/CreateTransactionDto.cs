using CashFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.DTOs
{
    public sealed record CreateTransactionDto(
    string Date,
    decimal Amount,
    TransactionType Type,
    string Description
    );
}
