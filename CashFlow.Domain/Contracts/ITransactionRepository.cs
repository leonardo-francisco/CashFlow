﻿using CashFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Domain.Contracts
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction, CancellationToken ct = default);
        Task<IReadOnlyList<Transaction>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
    }
}
