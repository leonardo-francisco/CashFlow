using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Infrastructure.Security
{
    public static class PollyPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> HttpRetryPolicy(int retryCount = 3) =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(retryCount, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        public static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy(
            int exceptionsAllowedBeforeBreaking = 5,
            int durationOfBreakSeconds = 30) =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(exceptionsAllowedBeforeBreaking, TimeSpan.FromSeconds(durationOfBreakSeconds));
    }
}
