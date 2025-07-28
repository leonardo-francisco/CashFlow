using CashFlow.Application.Contracts;
using CashFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.CQRS.Queries
{
    public sealed class GetDailyBalanceQueryHandler : IRequestHandler<GetDailyBalanceQuery, DailyBalanceDto>
    {
        private readonly IDailyBalanceService _dailyBalanceService;

        public GetDailyBalanceQueryHandler(IDailyBalanceService dailyBalanceService)
        {
            _dailyBalanceService = dailyBalanceService;
        }

        public async Task<DailyBalanceDto> Handle(GetDailyBalanceQuery request, CancellationToken cancellationToken)
        {
            return await _dailyBalanceService.GetDailyConsolidationAsync(request.Date);
        }
    }
}
