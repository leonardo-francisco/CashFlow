using AutoMapper;
using CashFlow.Application.Contracts;
using CashFlow.Application.DTOs;
using CashFlow.Application.Services;
using CashFlow.Domain.Contracts;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Events;
using MongoDB.Bson;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Tests.Application.Tests
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _repoMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly Mock<IMessageBus> _busMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            _service = new TransactionService(_repoMock.Object, _cacheMock.Object, _busMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldAddTransaction_AndPublishEvent_AndInvalidateCache()
        {
            // Arrange
            var dto = new TransactionDto { Date = DateOnly.FromDateTime(System.DateTime.Today) };
            var entity = new Transaction { Id = ObjectId.GenerateNewId(),UserId = null, Date = dto.Date, Amount = 100, Type = Domain.Enums.TransactionType.Credit };
            _mapperMock.Setup(m => m.Map<Transaction>(dto)).Returns(entity);
            _mapperMock.Setup(m => m.Map<TransactionDto>(entity)).Returns(dto);

            // Act
            var result = await _service.AddTransactionAsync(dto);

            // Assert
            _repoMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _busMock.Verify(b => b.PublishAsync("transactions.created", It.IsAny<TransactionCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task GetTransactionsByDateAsync_ShouldReturnFromCache_IfExists()
        {
            // Arrange
            var date = DateOnly.FromDateTime(System.DateTime.Today);
            var cacheKey = $"transactions:{date:yyyy-MM-dd}";
            var cached = new List<TransactionDto> { new TransactionDto { Date = date } };
            _cacheMock.Setup(c => c.GetAsync<List<TransactionDto>>(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(cached);

            // Act
            var result = await _service.GetTransactionsByDateAsync(date);

            // Assert
            Assert.Equal(cached, result);
            _repoMock.Verify(r => r.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
