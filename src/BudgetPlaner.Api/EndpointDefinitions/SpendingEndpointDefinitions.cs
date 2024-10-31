using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Constants.EndpointNames;
using BudgetPlaner.Api.DatabaseContext;
using BudgetPlaner.Api.Extensions;
using BudgetPlaner.Api.Mappers;
using BudgetPlaner.Api.Repository.UnitOfWork;
using BudgetPlaner.Models.Api;
using BudgetPlaner.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sqids;

namespace BudgetPlaner.Api.EndpointDefinitions;

public class SpendingEndpointDefinitions : IEndpointDefinition
{
    private const string BasePath = $"{EndpointNames.BudgetBasePath}/{EndpointNames.SpendingPath}";
    
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet(BasePath, GetSpendings)
            .WithTags(SwaggerTags.IncomeTag)
            .RequireAuthorization();

        app.MapGet(BasePath + "/{id}", GetSpending)
            .WithTags(SwaggerTags.IncomeTag)
            .RequireAuthorization();

        app.MapPost(BasePath, AddSpending)
            .WithTags(SwaggerTags.IncomeTag)
            .RequireAuthorization();

        app.MapPut(BasePath + "/{id}", UpdateSpending)
            .WithTags(SwaggerTags.IncomeTag)
            .RequireAuthorization();
        
        app.MapDelete(BasePath + "/{id}", DeleteSpending)
            .WithTags(SwaggerTags.IncomeTag)
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

        var category = await unitOfWork.Repository<IncomeEntity>()
            .FirstOrDefaultAsync(x => x.Id == idDecoded && x.UserId.Equals(userId));

        return category == null ? Results.BadRequest() : Results.Ok(category.MapToModel(sqidsEncoder));
    }

    private static async Task<IResult> AddSpending([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromBody] SpendingModel spendingModel)
    {
        var entity = spendingModel.MapToEntity();
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
        [FromServices] SqidsEncoder<int> sqidsEncoder, string id, [FromBody] SpendingModel spendingModel)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
        if (idDecoded == 0) return Results.BadRequest();

        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        await unitOfWork.Repository<SpendingEntity>()
            .UpdateAsync(x => x.Id == idDecoded && x.UserId.Equals(userId),
                prop =>
                    prop.SetProperty(c => c.Description, spendingModel.Description)
                        .SetProperty(c => c.CurrencyId, spendingModel.CurrencyId)
                        .SetProperty(c => c.CategoryId, spendingModel.CategoryId)
                        .SetProperty(c => c.Value, spendingModel.Value)
                        .SetProperty(c => c.ActualDateOfSpending, spendingModel.ActualDateOfSpending)
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

        await unitOfWork.Repository<SpendingEntity>()
            .DeleteAsync(x => x.Id == idDecoded && x.UserId.Equals(userId));

        return Results.NoContent();
    }

    public void DefineServices(IServiceCollection services)
    {
        
    }
}