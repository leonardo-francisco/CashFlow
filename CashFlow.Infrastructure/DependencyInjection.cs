using CashFlow.Application.Contracts;
using CashFlow.Domain.Contracts;
using CashFlow.Infrastructure.Cache;
using CashFlow.Infrastructure.Data;
using CashFlow.Infrastructure.MessageBus;
using CashFlow.Infrastructure.Repositories;
using CashFlow.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region MongoDB
            services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));
            services.AddSingleton<MongoDbContext>();
            #endregion

            #region Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });
            services.AddSingleton<ICacheService, RedisCacheService>();
            #endregion


            #region RabbitMQ
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
            services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
            #endregion

            #region Repositories
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            #endregion

            #region Polly / Resilience (HttpClient + Circuit Breaker)
            services.AddHttpClient("ResilientClient")
            .AddPolicyHandler(PollyPolicies.HttpRetryPolicy())
            .AddPolicyHandler(PollyPolicies.CircuitBreakerPolicy());
            #endregion

            return services;
        }
    }
}
