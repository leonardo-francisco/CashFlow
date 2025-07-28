using AutoMapper;
using CashFlow.Application.Contracts;
using CashFlow.Application.DTOs;
using CashFlow.Domain.Contracts;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICacheService _cache;
        private readonly IMessageBus _messageBus;
        private readonly IMapper _mapper;

        private static readonly TimeSpan CacheTTL = TimeSpan.FromMinutes(5);

        public TransactionService(
            ITransactionRepository transactionRepository,
            ICacheService cache,
            IMessageBus messageBus,
            IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _cache = cache;
            _messageBus = messageBus;
            _mapper = mapper;
        }

        public async Task<TransactionDto> AddTransactionAsync(TransactionDto dto, CancellationToken ct = default)
        {
            // Criar entidade
            var transaction = _mapper.Map<Transaction>(dto);
            await _transactionRepository.AddAsync(transaction, ct);

            // Invalida cache para o dia da transação
            string dailyCacheKey = $"transactions:{transaction.Date:yyyy-MM-dd}";
            await _cache.RemoveAsync(dailyCacheKey, ct);

            // Publica evento no RabbitMQ
            var evt = new TransactionCreatedEvent(transaction);
            await _messageBus.PublishAsync("transactions.created", evt, ct);

            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task<List<TransactionDto>> GetTransactionsByDateAsync(DateOnly date, CancellationToken ct = default)
        {
            string cacheKey = $"transactions:{date:yyyy-MM-dd}";
            var cached = await _cache.GetAsync<List<TransactionDto>>(cacheKey, ct);

            if (cached != null)
                return cached;

            var transactions = await _transactionRepository.GetByDateAsync(date, ct);
            var result = transactions.Select(_mapper.Map<TransactionDto>).ToList();

            await _cache.SetAsync(cacheKey, result, CacheTTL, ct);
            return result;
        }
    }
}
