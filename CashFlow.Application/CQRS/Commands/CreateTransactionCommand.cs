using CashFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.CQRS.Commands
{
    public sealed record CreateTransactionCommand(CreateTransactionDto Dto) : IRequest<TransactionDto>;
}
