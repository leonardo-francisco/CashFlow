using CashFlow.Application;
using CashFlow.Infrastructure;
using CashFlow.Shared.ScalarOpenApiConfiguration;
using CashFlow.TransactionAPI.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace CashFlow.TransactionAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            builder.Services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });
            OpenApiCustomGenerator.AddOpenApiCustom(builder.Services);

            var config = builder.Configuration;
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 var secret = config["Key:Secret"]!;
                 var issuer = config["Key:Issuer"];
                 var audience = config["Key:Audience"];

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidIssuer = issuer,

                     ValidateAudience = true,
                     ValidAudience = audience,

                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),

                     ValidateLifetime = true
                 };
             });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options
                    .WithTheme(ScalarTheme.BluePlanet)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            }

            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
