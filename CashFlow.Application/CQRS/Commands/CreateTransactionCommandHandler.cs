using AutoMapper;
using CashFlow.Application.Contracts;
using CashFlow.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.CQRS.Commands
{
    public sealed class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public CreateTransactionCommandHandler(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var dto = _mapper.Map<TransactionDto>(request.Dto);
            return await _transactionService.AddTransactionAsync(dto, cancellationToken);
        }
    }
}
