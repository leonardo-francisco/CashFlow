using CashFlow.Application.Contracts;
using CashFlow.Application.DTOs;
using CashFlow.Application.Mappers;
using CashFlow.Application.Services;
using CashFlow.Application.Validators;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            #region Services
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IDailyBalanceService, DailyBalanceService>();
            #endregion

            #region FluentValidation
            services.AddScoped<IValidator<TransactionDto>, TransactionValidator>();
            services.AddScoped<IValidator<CreateTransactionDto>, CreateTransactionValidator>();
            #endregion

            #region MediatR
            services.AddMediatR(Assembly.GetExecutingAssembly());
            #endregion

            #region AutoMapper
            services.AddAutoMapper(cfg => cfg.AddProfile<MapConfig>());
            #endregion

            return services;
        }
    }
}
