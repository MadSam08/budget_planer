﻿using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Constants.EndpointNames;
using BudgetPlaner.Application.Services.Budget;
using BudgetPlaner.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BudgetPlaner.Api.EndpointDefinitions;

public class BudgetEndpointDefinitions : IEndpointDefinition
{
    private const string BasePath = $"/{EndpointNames.ApiBasePath}/{EndpointNames.BudgetsPath}";
    
    public void DefineEndpoints(WebApplication app)
    {
        var budgets = app.MapGroup(BasePath).RequireAuthorization();

        // Budget CRUD operations
        budgets.MapPost("/", CreateBudget);
        budgets.MapGet("/", GetUserBudgets);
        budgets.MapGet("/{id:int}", GetBudgetById);
        budgets.MapPut("/{id:int}", UpdateBudget);
        budgets.MapDelete("/{id:int}", DeleteBudget);

        // Budget category operations
        budgets.MapPost($"/{{budgetId:int}}/{EndpointNames.CategoriesPath}", AddCategoryToBudget);
        budgets.MapPut($"/{EndpointNames.CategoriesPath}/{{categoryId:int}}", UpdateBudgetCategory);
        budgets.MapDelete($"/{EndpointNames.CategoriesPath}/{{categoryId:int}}", RemoveCategoryFromBudget);

        // Budget analysis
        budgets.MapGet($"/{{id:int}}/{EndpointNames.UtilizationPath}", GetBudgetUtilization);
        budgets.MapGet($"/{{id:int}}/{EndpointNames.OverBudgetPath}", GetOverBudgetCategories);
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddScoped<IBudgetService, BudgetService>();
    }

    private static async Task<IResult> CreateBudget(
        [FromBody] BudgetEntity budget,
        [FromServices] IBudgetService budgetService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var createdBudget = await budgetService.CreateBudgetAsync(budget, userId);
        return Results.Created($"{BasePath}/{createdBudget.Id}", createdBudget);
    }

    private static async Task<IResult> GetUserBudgets(
        [FromServices] IBudgetService budgetService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var budgets = await budgetService.GetUserBudgetsAsync(userId);
        return Results.Ok(budgets);
    }

    private static async Task<IResult> GetBudgetById(
        int id,
        [FromServices] IBudgetService budgetService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var budget = await budgetService.GetBudgetByIdAsync(id, userId);
        return budget == null ? Results.NotFound() : Results.Ok(budget);
    }

    private static async Task<IResult> UpdateBudget(
        int id,
        [FromBody] BudgetEntity budget,
        [FromServices] IBudgetService budgetService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        budget.Id = id;
        var updatedBudget = await budgetService.UpdateBudgetAsync(budget, userId);
        return Results.Ok(updatedBudget);
    }

    private static async Task<IResult> DeleteBudget(
        int id,
        [FromServices] IBudgetService budgetService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var deleted = await budgetService.DeleteBudgetAsync(id, userId);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> AddCategoryToBudget(
        int budgetId,
        [FromBody] AddCategoryToBudgetRequest request,
        [FromServices] IBudgetService budgetService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var budgetCategory = await budgetService.AddCategoryToBudgetAsync(
            budgetId, request.CategoryId, request.AllocatedAmount, userId);
        return Results.Created($"{BasePath}/{EndpointNames.CategoriesPath}/{budgetCategory.Id}", budgetCategory);
    }

    private static async Task<IResult> UpdateBudgetCategory(
        int categoryId,
        [FromBody] UpdateBudgetCategoryRequest request,
        [FromServices] IBudgetService budgetService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var budgetCategory = await budgetService.UpdateBudgetCategoryAsync(
            categoryId, request.AllocatedAmount, userId);
        return Results.Ok(budgetCategory);
    }

    private static async Task<IResult> RemoveCategoryFromBudget(
        int categoryId,
        [FromServices] IBudgetService budgetService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var removed = await budgetService.RemoveCategoryFromBudgetAsync(categoryId, userId);
        return removed ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> GetBudgetUtilization(
        int id,
        [FromServices] IBudgetService budgetService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var utilization = await budgetService.GetBudgetUtilizationAsync(id, userId);
        return Results.Ok(new { BudgetId = id, UtilizationPercentage = utilization });
    }

    private static async Task<IResult> GetOverBudgetCategories(
        int id,
        [FromServices] IBudgetService budgetService,
        ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var categories = await budgetService.GetOverBudgetCategoriesAsync(id, userId);
        return Results.Ok(categories);
    }
}

public record AddCategoryToBudgetRequest(int CategoryId, decimal AllocatedAmount);
public record UpdateBudgetCategoryRequest(decimal AllocatedAmount);