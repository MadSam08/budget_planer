using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Constants;
using BudgetPlaner.Application.Services.Insights;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BudgetPlaner.Api.Constants.EndpointNames;

namespace BudgetPlaner.Api.EndpointDefinitions;

public class FinancialInsightsEndpointDefinitions : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        // Get insights
        app.MapGet(ApiEndpoints.Insights.GetAll, GetUserInsights)
            .WithTags(SwaggerTags.FinancialInsightTag)
            .RequireAuthorization();
        app.MapGet(ApiEndpoints.Insights.Get, GetInsightById)
            .WithTags(SwaggerTags.FinancialInsightTag)
            .RequireAuthorization();
        app.MapPost(ApiEndpoints.Insights.Create, GenerateMonthlyInsights)            
            .WithTags(SwaggerTags.FinancialInsightTag)
            .RequireAuthorization();

        // Insight actions
        app.MapPut(ApiEndpoints.Insights.MarkAsRead, MarkInsightAsRead).WithTags(SwaggerTags.FinancialInsightTag).RequireAuthorization();
        app.MapPut(ApiEndpoints.Insights.MarkActionTaken, MarkInsightActionTaken).WithTags(SwaggerTags.FinancialInsightTag).RequireAuthorization();

        // Specific analysis endpoints
        app.MapGet(ApiEndpoints.Insights.SpendingPatterns, AnalyzeSpendingPatterns).WithTags(SwaggerTags.FinancialInsightTag).RequireAuthorization();
        app.MapGet(ApiEndpoints.Insights.SavingsOpportunities, GenerateSavingsOpportunities).WithTags(SwaggerTags.FinancialInsightTag).RequireAuthorization();
        app.MapGet(ApiEndpoints.Insights.BudgetPerformance, AnalyzeBudgetPerformance).WithTags(SwaggerTags.FinancialInsightTag).RequireAuthorization();
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddScoped<IFinancialInsightService, FinancialInsightService>();
    }

    private static async Task<IResult> GetUserInsights(
        [FromServices] IFinancialInsightService insightService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var insights = await insightService.GetUserInsightsAsync(userId);
        return Results.Ok(insights);
    }

    private static async Task<IResult> GetInsightById(
        int id,
        [FromServices] IFinancialInsightService insightService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        // This would need to be implemented in the service
        // For now, return NotFound as placeholder
        return Results.NotFound();
    }

    private static async Task<IResult> GetUnreadInsights(
        [FromServices] IFinancialInsightService insightService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var insights = await insightService.GetUserInsightsAsync(userId, unreadOnly: true);
        return Results.Ok(insights);
    }

    private static async Task<IResult> GenerateMonthlyInsights(
        [FromServices] IFinancialInsightService insightService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var insights = await insightService.GenerateMonthlyInsightsAsync(userId);
        return Results.Ok(insights);
    }

    private static async Task<IResult> MarkInsightAsRead(
        int id,
        [FromServices] IFinancialInsightService insightService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var success = await insightService.MarkInsightAsReadAsync(id, userId);
        return success ? Results.Ok() : Results.NotFound();
    }

    private static async Task<IResult> MarkInsightActionTaken(
        int id,
        [FromServices] IFinancialInsightService insightService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var success = await insightService.MarkInsightActionTakenAsync(id, userId);
        return success ? Results.Ok() : Results.NotFound();
    }

    private static async Task<IResult> AnalyzeSpendingPatterns(
        [FromServices] IFinancialInsightService insightService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var insights = await insightService.AnalyzeSpendingPatternsAsync(userId);
        return Results.Ok(insights);
    }

    private static async Task<IResult> GenerateSavingsOpportunities(
        [FromServices] IFinancialInsightService insightService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var insights = await insightService.GenerateSavingsOpportunitiesAsync(userId);
        return Results.Ok(insights);
    }

    private static async Task<IResult> AnalyzeBudgetPerformance(
        [FromServices] IFinancialInsightService insightService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var insights = await insightService.AnalyzeBudgetPerformanceAsync(userId);
        return Results.Ok(insights);
    }

    private static async Task<IResult> GenerateLoanOptimization(
        int loanId,
        [FromServices] IFinancialInsightService insightService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var insight = await insightService.GenerateLoanOptimizationSuggestionAsync(loanId, userId);
        return insight != null ? Results.Ok(insight) : Results.NotFound();
    }
}