using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace BudgetPlaner.Api.Bootstrap;

public class SwaggerEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.UseSwagger(options => { options.RouteTemplate = app.Configuration["Swagger:RouteTemplate"]; });
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = app.Configuration["Swagger:Path"];
            options.DisplayRequestDuration();
            options.ConfigObject.DocExpansion = DocExpansion.None;
            options.ConfigObject.DisplayRequestDuration = true;
            options.ConfigObject.Filter = string.Empty;

            // Allow "Try it out" in development only.
            if (!app.Environment.IsDevelopment()) options.SupportedSubmitMethods();
        });
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Budget Planner", Version = "v1" });
        });
    }
}
