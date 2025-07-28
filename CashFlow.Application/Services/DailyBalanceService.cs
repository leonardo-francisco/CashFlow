using CashFlow.Application.Contracts;
using CashFlow.Application.DTOs;
using CashFlow.Domain.Contracts;
using CashFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.Services
{
    public class DailyBalanceService : IDailyBalanceService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICacheService _cache;
        private readonly IMessageBus _messageBus;
        private static readonly TimeSpan CacheTTL = TimeSpan.FromMinutes(10);

        public DailyBalanceService(
            ITransactionRepository transactionRepository,
            ICacheService cache,
            IMessageBus messageBus)
        {
            _transactionRepository = transactionRepository;
            _cache = cache;
            _messageBus = messageBus;
        }

        public async Task<DailyBalanceDto> GetDailyConsolidationAsync(DateOnly date)
        {
            string cacheKey = $"dailybalance:{date:yyyy-MM-dd}";

            // Tenta buscar do cache
            var cached = await _cache.GetAsync<DailyBalanceDto>(cacheKey);
            if (cached is not null)
                return cached;

            // Calcula se não estiver no cache
            var transactions = await _transactionRepository.GetByDateAsync(date);
            var dto = new DailyBalanceDto(
                Date: date,
                TotalCredits: transactions.Where(t => t.Type == TransactionType.Credit).Sum(t => t.Amount),
                TotalDebits: transactions.Where(t => t.Type == TransactionType.Debit).Sum(t => t.Amount),
                Balance: transactions.Sum(t => t.Type == TransactionType.Credit ? t.Amount : -t.Amount)
            );

            // Grava no cache
            await _cache.SetAsync(cacheKey, dto, CacheTTL);

            // Publica um evento no Rabbit
            await _messageBus.PublishAsync("dailybalance.calculated", dto);

            return dto;
        }
    }
}
