﻿using BudgetPlaner.Api.Bootstrap;

namespace BudgetPlaner.Api.Extensions;

public static class EndpointDefinitionExtensions
{
    public static void AddEndpointDefinitions(
        this IServiceCollection services, params Type[] scanMarkers)
    {
        var endpointDefinitions = new List<IEndpointDefinition>();

        foreach (var marker in scanMarkers)
        {
            endpointDefinitions.AddRange(
                marker.Assembly.ExportedTypes
                    .Where(x => typeof(IEndpointDefinition).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
                    .Select(Activator.CreateInstance).Cast<IEndpointDefinition>()
                );
        }

        foreach (var endpointDefinition in endpointDefinitions)
        {
            endpointDefinition.DefineServices(services);
        }

        services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);
    }

    public static void UseEndpointDefinitions(this WebApplication app)
    {
        var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();

        foreach (var endpointDefinition in definitions)
        {
            endpointDefinition.DefineEndpoints(app);
        }
    }

    // public static RouteHandlerBuilder AddValidationFilter<TRequest>(this RouteHandlerBuilder builder)
    //     where TRequest : class
    //     => builder.AddEndpointFilter<ValidationFilter<TRequest>>();
}
