using CashFlow.Application.CQRS.Queries;
using CashFlow.Application.DTOs;
using CashFlow.DailyBalanceAPI.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Tests.API.Tests
{
    public class DailyBalanceControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DailyBalanceController _controller;

        public DailyBalanceControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new DailyBalanceController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetDailyBalance_ReturnsBadRequest_WhenDateIsInvalid()
        {
            // Act
            var result = await _controller.GetDailyBalance("invalid-date", default);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("A data deve estar no formato dd-MM-yyyy.", badRequest.Value);
        }

        [Fact]
        public async Task GetDailyBalance_ReturnsOk_WhenDateIsValid()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Today);
            var dateStr = date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            var dailyBalance = new DailyBalanceDto(date, 100, 50, 50);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDailyBalanceQuery>(), default)).ReturnsAsync(dailyBalance);

            // Act
            var result = await _controller.GetDailyBalance(dateStr, default);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dailyBalance, okResult.Value);
        }
    }
}
