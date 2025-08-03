using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Constants;
using BudgetPlaner.Api.Constants.EndpointNames;
using BudgetPlaner.Api.Extensions;
using BudgetPlaner.Api.Mappers;
using BudgetPlaner.Contracts.Api;
using BudgetPlaner.Contracts.Api.Loan;
using BudgetPlaner.Domain;
using BudgetPlaner.Infrastructure.DatabaseContext;
using BudgetPlaner.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sqids;

namespace BudgetPlaner.Api.EndpointDefinitions;

public class SpendingEndpointDefinitions : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet(ApiEndpoints.Expenses.GetAll, GetSpendings)
            .WithTags(SwaggerTags.SpendingTag)
            .RequireAuthorization();

        app.MapGet(ApiEndpoints.Expenses.Get, GetSpending)
            .WithTags(SwaggerTags.SpendingTag)
            .RequireAuthorization();

        app.MapPost(ApiEndpoints.Expenses.Create, AddSpending)
            .WithTags(SwaggerTags.SpendingTag)
            .RequireAuthorization();

        app.MapPut(ApiEndpoints.Expenses.Update, UpdateSpending)
            .WithTags(SwaggerTags.SpendingTag)
            .RequireAuthorization();
        
        app.MapDelete(ApiEndpoints.Expenses.Delete, DeleteSpending)
            .WithTags(SwaggerTags.SpendingTag)
            .RequireAuthorization();
    }
    
    private static async Task<IResult> GetSpendings([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder, CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        var response = await unitOfWork.Repository<SpendingEntity>()
            .Where(x => x.UserId.Equals(userId)).ToListAsync(cancellationToken: cancellationToken);

        return Results.Ok(response.MapToModel(sqidsEncoder));
    }

    private static async Task<IResult> GetSpending([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder, string id)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();

        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        var spending = await unitOfWork.Repository<SpendingEntity>()
            .FirstOrDefaultAsync(x => x.Id == idDecoded && x.UserId.Equals(userId));

        return spending == null ? Results.BadRequest() : Results.Ok(spending.MapToModel(sqidsEncoder));
    }

    private static async Task<IResult> AddSpending([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder,
        [FromBody] SpendingRequest spendingRequest)
    {
        var entity = spendingRequest.MapToEntity(sqidsEncoder);
        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        entity.UserId = userId;
        entity.CreateDate = DateTime.UtcNow;
        entity.UpdateDate = DateTime.UtcNow;

        await unitOfWork.Repository<SpendingEntity>().AddAsync(entity);
        await unitOfWork.Complete();
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateSpending([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder, string id, [FromBody] SpendingRequest spendingRequest)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
        if (idDecoded == 0) return Results.BadRequest();

        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        await unitOfWork.Repository<SpendingEntity>()
            .UpdateAsync(x => x.Id == idDecoded && x.UserId.Equals(userId),
                prop =>
                    prop.SetProperty(c => c.Description, spendingRequest.Description)
                        .SetProperty(c => c.CurrencyId, sqidsEncoder.Decode(spendingRequest.CurrencyId).SingleOrDefault())
                        .SetProperty(c => c.CategoryId, sqidsEncoder.Decode(spendingRequest.CategoryId).SingleOrDefault())
                        .SetProperty(c => c.Value, spendingRequest.Value)
                        .SetProperty(c => c.ActualDateOfSpending, spendingRequest.ActualDateOfSpending)
                        .SetProperty(c => c.UpdateDate, DateTime.UtcNow));

        return Results.NoContent();
    }
    
    private static async Task<IResult> DeleteSpending([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder, string id)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
        if (idDecoded == 0) return Results.BadRequest();

        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        var deletedCount = await unitOfWork.Repository<SpendingEntity>()
            .ExecuteDeleteAsync(x => x.Id == idDecoded && x.UserId.Equals(userId));

        return deletedCount > 0 ? Results.NoContent() : Results.NotFound();
    }

    public void DefineServices(IServiceCollection services)
    {
        
    }
}