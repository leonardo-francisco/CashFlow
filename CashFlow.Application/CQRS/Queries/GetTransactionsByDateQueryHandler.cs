using CashFlow.Application.Contracts;
using CashFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.CQRS.Queries
{
    public sealed class GetTransactionsByDateQueryHandler : IRequestHandler<GetTransactionsByDateQuery, IEnumerable<TransactionDto>>
    {
        private readonly ITransactionService _transactionService;

        public GetTransactionsByDateQueryHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<IEnumerable<TransactionDto>> Handle(GetTransactionsByDateQuery request, CancellationToken cancellationToken)
        {
            return await _transactionService.GetTransactionsByDateAsync(request.Date, cancellationToken);
        }
    }
}
