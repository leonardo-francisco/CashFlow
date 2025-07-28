using CashFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.CQRS.Queries
{
    public sealed record GetTransactionsByDateQuery(DateOnly Date) : IRequest<IEnumerable<TransactionDto>>;
}
