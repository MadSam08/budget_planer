using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Application.Services.Insights;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BudgetPlaner.Api.EndpointDefinitions;

public class FinancialInsightsEndpointDefinitions : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        var insights = app.MapGroup("/api/insights").RequireAuthorization();

        // Get insights
        insights.MapGet("/", GetUserInsights);
        insights.MapGet("/unread", GetUnreadInsights);
        insights.MapGet("/generate", GenerateMonthlyInsights);

        // Insight actions
        insights.MapPut("/{id:int}/read", MarkInsightAsRead);
        insights.MapPut("/{id:int}/action-taken", MarkInsightActionTaken);

        // Specific analysis endpoints
        insights.MapGet("/spending-patterns", AnalyzeSpendingPatterns);
        insights.MapGet("/savings-opportunities", GenerateSavingsOpportunities);
        insights.MapGet("/budget-performance", AnalyzeBudgetPerformance);
        insights.MapGet("/loan-optimization/{loanId:int}", GenerateLoanOptimization);
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
 