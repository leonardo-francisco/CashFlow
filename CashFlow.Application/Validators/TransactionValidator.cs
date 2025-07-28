using CashFlow.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CashFlow.Application.Validators
{
    public class TransactionValidator : AbstractValidator<TransactionDto>
    {
        public TransactionValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("O valor da transação deve ser positivo.");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("O tipo de transação é inválido.");

            RuleFor(x => x.Date)
                .Must(BeValidDate)
                .WithMessage("A data da transação não pode ser futura.");
        }

        private bool BeValidDate(DateOnly date)
        {
            return date != default && date <= DateOnly.FromDateTime(DateTime.Today);
        }
    }
}
