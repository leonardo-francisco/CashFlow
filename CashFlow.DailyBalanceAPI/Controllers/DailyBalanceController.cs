using CashFlow.Application.CQRS.Queries;
using CashFlow.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CashFlow.DailyBalanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DailyBalanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DailyBalanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retorna o saldo consolidado do dia informado.
        /// </summary>
        [HttpGet("{date}")]
        [ProducesResponseType(typeof(DailyBalanceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDailyBalance([FromRoute] string date, CancellationToken cancellationToken)
        {
            if (!DateOnly.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                return BadRequest("A data deve estar no formato dd-MM-yyyy.");

            var result = await _mediator.Send(new GetDailyBalanceQuery(parsedDate), cancellationToken);
            return Ok(result);
        }
    }
}
