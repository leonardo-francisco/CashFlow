using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CashFlow.TransactionAPI.Configurations
{
    public static class OpenApiCustomGenerator
    {
        public static void AddOpenApiCustom(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddOperationTransformer((operation, context, ct) =>
                {
                    // foreach exception in `CustomExceptionHandler.cs` we need to add it to possible return types of an operation
                    AddResponse<ValidationException>(operation, StatusCodes.Status400BadRequest);
                    AddResponse<UnauthorizedAccessException>(operation, StatusCodes.Status401Unauthorized);
                    AddResponse<InvalidOperationException>(operation, StatusCodes.Status403Forbidden);


                    return Task.CompletedTask;
                });

                var openApiMode = Environment.GetEnvironmentVariable("OPENAPI_MODE");

                options.AddDocumentTransformer((doc, context, cancellationToken) =>
                {
                    doc.Info.Title = "CashFlowApp API";
                    doc.Info.Description = "Control your daily cash flow";

                    doc.Components = doc.Components ?? new OpenApiComponents();

                    AddResponseSchema<ValidationException>(doc, typeof(ValidationProblemDetails));
                    AddResponseSchema<UnauthorizedAccessException>(doc);
                    AddResponseSchema<InvalidOperationException>(doc);

                    // Configuração de URL dependendo do modo
                    if (openApiMode == "Docker")
                    {
                        doc.Servers = new List<OpenApiServer>
                        {
                            new OpenApiServer { Url = "http://localhost:5004" }
                        };
                    }
                    else // Localhost
                    {
                        doc.Servers = new List<OpenApiServer>
                        {
                            new OpenApiServer { Url = "https://localhost:44345" }
                        };
                    }

                    return Task.CompletedTask;
                });
            });
        }

        // Helper method to add a response to an operation
        private static void AddResponse<T>(OpenApiOperation operation, int statusCode) where T : class
        {
            var responseType = typeof(T);
            var responseTypeName = responseType.Name;

            // Check if the response for the status code already exists
            if (operation.Responses.ContainsKey(statusCode.ToString()))
            {
                return;
            }

            // Create an OpenApiResponse and set the content to reference the exception schema
            operation.Responses[statusCode.ToString()] = new OpenApiResponse
            {
                Description = $"{responseTypeName} - {statusCode}",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = responseTypeName
                            }
                        }
                    }
                }
            };
        }

        // Helper method to add a response schema to the OpenAPI document
        private static void AddResponseSchema<T>(OpenApiDocument doc, Type? responseType = null)
        {
            var exceptionType = typeof(T);
            var responseTypeName = exceptionType.Name;

            // the default response type of errors / exceptions --> check: `CustomExceptionHandler.cs`
            responseType = responseType ?? typeof(ProblemDetails);

            // Define the schema for the exception type if it doesn't already exist
            if (doc.Components.Schemas.ContainsKey(responseTypeName))
            {
                return;
            }

            // Dynamically build the schema based on the properties of T
            var properties = responseType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(
                    prop => prop.Name,
                    prop => new OpenApiSchema
                    {
                        Type = GetOpenApiType(prop.PropertyType),
                        Description = $"Property of type {prop.PropertyType.Name}"
                    }
                );

            // Add the schema to the OpenAPI document components
            doc.Components.Schemas[responseTypeName] = new OpenApiSchema
            {
                Type = "object",
                Properties = properties
            };
        }

        // Helper method to map .NET types to OpenAPI types
        private static string GetOpenApiType(Type type)
        {
            return type == typeof(string) ? "string" :
                   type == typeof(int) || type == typeof(long) ? "integer" :
                   type == typeof(bool) ? "boolean" :
                   type == typeof(float) || type == typeof(double) || type == typeof(decimal) ? "number" :
                   "string"; // Fallback for complex types

        }
    }
}
