using CashFlow.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.Contracts
{
    public interface ITransactionService
    {
        Task<TransactionDto> AddTransactionAsync(TransactionDto transaction, CancellationToken ct);
        Task<List<TransactionDto>> GetTransactionsByDateAsync(DateOnly date, CancellationToken ct);
    }
}
