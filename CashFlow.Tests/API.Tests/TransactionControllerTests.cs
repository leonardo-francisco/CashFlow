using CashFlow.Application.CQRS.Commands;
using CashFlow.Application.CQRS.Queries;
using CashFlow.Application.DTOs;
using CashFlow.Domain.Enums;
using CashFlow.TransactionAPI.Controllers;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Tests.API.Tests
{
    public class TransactionControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IValidator<CreateTransactionDto>> _validatorMock;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _validatorMock = new Mock<IValidator<CreateTransactionDto>>();
            _controller = new TransactionController(_mediatorMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var dto = new CreateTransactionDto("27-07-2025", 100.00m, default, "Teste Mock");

            var validatorMock = new Mock<IValidator<CreateTransactionDto>>();
            validatorMock
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
                {
                new ValidationFailure("Type", "Type is required")
                }));

            var mediatorMock = new Mock<IMediator>();

            var controller = new TransactionController(mediatorMock.Object, validatorMock.Object);

            // Act
            var result = await controller.CreateTransaction(dto, default);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(badRequest.Value);
            Assert.Contains("Type is required", errors);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsCreated_WhenValid()
        {
            // Arrange
            var dto = new CreateTransactionDto("27-07-2025", 100.00m, TransactionType.Credit, "Teste Mock");
            var validationResult = new FluentValidation.Results.ValidationResult();

            var transactionDto = new TransactionDto { Id = "1", Type = "Crédito", Date = DateOnly.FromDateTime(DateTime.Today) };

            _validatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(validationResult);
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateTransactionCommand>(), default)).ReturnsAsync(transactionDto);

            // Act
            var result = await _controller.CreateTransaction(dto, default);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(transactionDto, created.Value);
        }

        [Fact]
        public async Task GetTransactionsByDate_ReturnsBadRequest_WhenDateIsInvalid()
        {
            // Act
            var result = await _controller.GetTransactionsByDate("invalid-date", default);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("A data deve estar no formato dd-MM-yyyy.", badRequest.Value);
        }

        [Fact]
        public async Task GetTransactionsByDate_ReturnsOk_WhenDateIsValid()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Today);
            var dateStr = date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            var transactions = new List<TransactionDto>
            {
                new TransactionDto { Id = "1", Type = "Crédito", Date = date }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTransactionsByDateQuery>(), default)).ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetTransactionsByDate(dateStr, default);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transactions, okResult.Value);
        }
    }
}
