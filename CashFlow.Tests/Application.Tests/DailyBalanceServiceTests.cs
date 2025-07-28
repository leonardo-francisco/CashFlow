using CashFlow.Application.Contracts;
using CashFlow.Application.DTOs;
using CashFlow.Application.Services;
using CashFlow.Domain.Contracts;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using MongoDB.Bson;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Tests.Application.Tests
{
    public class DailyBalanceServiceTests
    {
        private readonly Mock<ITransactionRepository> _repoMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly Mock<IMessageBus> _busMock = new();
        private readonly DailyBalanceService _service;

        public DailyBalanceServiceTests()
        {
            _service = new DailyBalanceService(_repoMock.Object, _cacheMock.Object, _busMock.Object);
        }

        [Fact]
        public async Task GetDailyConsolidationAsync_ReturnsFromCache_IfExists()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Today);
            var cached = new DailyBalanceDto(date,100,50,50);
           
            _cacheMock.Setup(c => c.GetAsync<DailyBalanceDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(cached);

            // Act
            var result = await _service.GetDailyConsolidationAsync(date);

            // Assert
            Assert.Equal(cached, result);
            _repoMock.Verify(r => r.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetDailyConsolidationAsync_CalculatesAndPublishes_IfNotCached()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Today);

            _cacheMock.Setup(c => c.GetAsync<DailyBalanceDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((DailyBalanceDto)null!);

            var transactions = new List<Transaction>
            {
                new Transaction {Id = ObjectId.GenerateNewId(), Amount = 100, Type = TransactionType.Credit, Date = date, UserId = "user1" },
                new Transaction {Id = ObjectId.GenerateNewId(), Amount = 50, Type = TransactionType.Debit, Date = date, UserId = "user1" }
            };
            _repoMock.Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>())).ReturnsAsync(transactions);

            // Act
            var result = await _service.GetDailyConsolidationAsync(date);

            // Assert
            Assert.Equal(100, result.TotalCredits);
            Assert.Equal(50, result.TotalDebits);
            Assert.Equal(50, result.Balance);
            _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), result, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
            _busMock.Verify(b => b.PublishAsync("dailybalance.calculated", result, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
