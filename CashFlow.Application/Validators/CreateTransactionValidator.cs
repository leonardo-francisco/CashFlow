using CashFlow.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application.Validators
{
    public class CreateTransactionValidator : AbstractValidator<CreateTransactionDto>
    {
        private const string BrDateFormat = "dd-MM-yyyy";

        public CreateTransactionValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("O valor da transação deve ser positivo.");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("O tipo de transação é inválido.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("A data é obrigatória.")
                .Must(date => DateOnly.TryParseExact(date, BrDateFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                .WithMessage("A data deve estar no formato dd-MM-yyyy.");
        }
    }
}
