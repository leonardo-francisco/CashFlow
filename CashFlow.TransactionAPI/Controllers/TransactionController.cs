using CashFlow.Application.CQRS.Commands;
using CashFlow.Application.CQRS.Queries;
using CashFlow.Application.DTOs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CashFlow.TransactionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<CreateTransactionDto> _validator;

        public TransactionController(IMediator mediator, IValidator<CreateTransactionDto> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        /// <summary>
        /// Cria uma nova transação.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var result = await _mediator.Send(new CreateTransactionCommand(dto), cancellationToken);
            return CreatedAtAction(nameof(GetTransactionsByDate), new { date = result.Date }, result);
        }

        /// <summary>
        /// Retorna todas as transações de uma data específica.
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactionsByDate([FromRoute] string date, CancellationToken cancellationToken)
        {
            if (!DateOnly.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                return BadRequest("A data deve estar no formato dd-MM-yyyy.");

            var result = await _mediator.Send(new GetTransactionsByDateQuery(parsedDate), cancellationToken);
            return Ok(result);
        }
    }
}
