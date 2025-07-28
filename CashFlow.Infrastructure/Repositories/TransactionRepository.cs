using CashFlow.Domain.Contracts;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Infrastructure.Repositories
{
    public sealed class TransactionRepository(MongoDbContext context) : ITransactionRepository
    {
        private readonly IMongoCollection<Transaction> _collection = context.Transactions;

        public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
        {
            await _collection.InsertOneAsync(transaction);
        }

        public async Task<IReadOnlyList<Transaction>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
        {
            var endOfDay = date.AddDays(1);

            var filter = Builders<Transaction>.Filter.And(
                Builders<Transaction>.Filter.Gte(t => t.Date, date),
                Builders<Transaction>.Filter.Lte(t => t.Date, endOfDay)
            );
            return await _collection.Find(filter).ToListAsync();
        }
    }
}
