using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Constants;
using BudgetPlaner.Application.Services.Budget;
using BudgetPlaner.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BudgetPlaner.Api.Constants.EndpointNames;
using BudgetPlaner.Api.Extensions;
using BudgetPlaner.Api.Mappers;
using BudgetPlaner.Contracts.Api.Budget;
using Sqids;

namespace BudgetPlaner.Api.EndpointDefinitions;

public class BudgetEndpointDefinitions : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        // Budget CRUD operations
        app.MapGet(ApiEndpoints.Budgets.GetAll, GetUserBudgets)
            .WithTags(SwaggerTags.BudgetTag).RequireAuthorization();
        app.MapGet(ApiEndpoints.Budgets.Get, GetBudgetById).WithTags(SwaggerTags.BudgetTag).RequireAuthorization();
        app.MapPost(ApiEndpoints.Budgets.Create, CreateBudget).WithTags(SwaggerTags.BudgetTag).RequireAuthorization();
        app.MapPut(ApiEndpoints.Budgets.Update, UpdateBudget).WithTags(SwaggerTags.BudgetTag).RequireAuthorization();
        app.MapDelete(ApiEndpoints.Budgets.Delete, DeleteBudget).WithTags(SwaggerTags.BudgetTag).RequireAuthorization();

        // Budget category operations
        app.MapPost(ApiEndpoints.Budgets.AddCategory, AddCategoryToBudget).WithTags(SwaggerTags.BudgetTag)
            .RequireAuthorization();
        app.MapPut(ApiEndpoints.Budgets.UpdateCategory, UpdateBudgetCategory).WithTags(SwaggerTags.BudgetTag)
            .RequireAuthorization();
        app.MapDelete(ApiEndpoints.Budgets.RemoveCategory, RemoveCategoryFromBudget).WithTags(SwaggerTags.BudgetTag)
            .RequireAuthorization();

        // Budget analysis
        app.MapGet(ApiEndpoints.Budgets.GetUtilization, GetBudgetUtilization).WithTags(SwaggerTags.BudgetTag)
            .RequireAuthorization();
        app.MapGet(ApiEndpoints.Budgets.GetOverBudget, GetOverBudgetCategories).WithTags(SwaggerTags.BudgetTag)
            .RequireAuthorization();
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddScoped<IBudgetService, BudgetService>();
    }

    private static async Task<IResult> CreateBudget(
        [FromBody] BudgetRequest budget,
        [FromServices] IBudgetService budgetService,
        [FromServices] SqidsEncoder<int> sqidsEncoder, 
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var createdBudget = await budgetService.CreateBudgetAsync(budget.MapToEntity(sqidsEncoder), userId);
        return Results.Created($"/api/budgets/{createdBudget.Id}", createdBudget);
    }

    private static async Task<IResult> GetUserBudgets(
        [FromServices] IBudgetService budgetService,
        [FromServices] SqidsEncoder<int> sqidsEncoder,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var budgets = await budgetService.GetUserBudgetsAsync(userId);
        return Results.Ok(budgets.MapToResponse(sqidsEncoder));
    }

    private static async Task<IResult> GetBudgetById(
        int id,
        [FromServices] IBudgetService budgetService,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var budget = await budgetService.GetBudgetByIdAsync(id, userId);
        return budget == null ? Results.NotFound() : Results.Ok(budget);
    }

    private static async Task<IResult> UpdateBudget(
        int id,
        [FromBody] BudgetEntity budget,
        [FromServices] IBudgetService budgetService,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        budget.Id = id;
        var updatedBudget = await budgetService.UpdateBudgetAsync(budget, userId);
        return Results.Ok(updatedBudget);
    }

    private static async Task<IResult> DeleteBudget(
        int id,
        [FromServices] IBudgetService budgetService,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var deleted = await budgetService.DeleteBudgetAsync(id, userId);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> AddCategoryToBudget(
        int budgetId,
        [FromBody] AddCategoryToBudgetRequest request,
        [FromServices] IBudgetService budgetService,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var budgetCategory = await budgetService.AddCategoryToBudgetAsync(
            budgetId, request.CategoryId, request.AllocatedAmount, userId);
        return Results.Created($"/api/budgets/categories/{budgetCategory.Id}", budgetCategory);
    }

    private static async Task<IResult> UpdateBudgetCategory(
        int categoryId,
        [FromBody] UpdateBudgetCategoryRequest request,
        [FromServices] IBudgetService budgetService,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var budgetCategory = await budgetService.UpdateBudgetCategoryAsync(
            categoryId, request.AllocatedAmount, userId);
        return Results.Ok(budgetCategory);
    }

    private static async Task<IResult> RemoveCategoryFromBudget(
        int categoryId,
        [FromServices] IBudgetService budgetService,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var removed = await budgetService.RemoveCategoryFromBudgetAsync(categoryId, userId);
        return removed ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> GetBudgetUtilization(
        int id,
        [FromServices] IBudgetService budgetService,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var utilization = await budgetService.GetBudgetUtilizationAsync(id, userId);
        return Results.Ok(new { BudgetId = id, UtilizationPercentage = utilization });
    }

    private static async Task<IResult> GetOverBudgetCategories(
        int id,
        [FromServices] IBudgetService budgetService,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        var categories = await budgetService.GetOverBudgetCategoriesAsync(id, userId);
        return Results.Ok(categories);
    }
}

public record AddCategoryToBudgetRequest(int CategoryId, decimal AllocatedAmount);

public record UpdateBudgetCategoryRequest(decimal AllocatedAmount);