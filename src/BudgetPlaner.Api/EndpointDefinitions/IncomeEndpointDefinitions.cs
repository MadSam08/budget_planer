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

public class IncomeEndpointDefinitions : IEndpointDefinition
{
    private const string BasePath = $"{EndpointNames.BudgetBasePath}/{EndpointNames.IncomePath}";

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet(BasePath, GetIncomes)
            .WithTags(SwaggerTags.IncomeTag)
            .RequireAuthorization();

        app.MapGet(BasePath + "/{id}", GetIncome)
            .WithTags(SwaggerTags.IncomeTag)
            .RequireAuthorization();

        app.MapPost(BasePath, AddIncome)
            .WithTags(SwaggerTags.IncomeTag)
            .RequireAuthorization();

        app.MapPut(BasePath + "/{id}", UpdateIncome)
            .WithTags(SwaggerTags.IncomeTag)
            .RequireAuthorization();
        
        app.MapDelete(BasePath + "/{id}", DeleteIncome)
            .WithTags(SwaggerTags.IncomeTag)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetIncomes([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder, CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        var response = await unitOfWork.Repository<IncomeEntity>()
            .Where(x => x.UserId.Equals(userId)).ToListAsync(cancellationToken: cancellationToken);

        return Results.Ok(response.MapToModel(sqidsEncoder));
    }

    private static async Task<IResult> GetIncome([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
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

    private static async Task<IResult> AddIncome([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromBody] IncomeModel incomeModel)
    {
        var entity = incomeModel.MapToEntity();
        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        entity.UserId = userId;
        entity.CreateDate = DateTime.UtcNow;
        entity.UpdateDate = DateTime.UtcNow;

        await unitOfWork.Repository<IncomeEntity>().AddAsync(entity);
        await unitOfWork.Complete();
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateIncome([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder, string id, [FromBody] IncomeModel categoryModel)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
        if (idDecoded == 0) return Results.BadRequest();

        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        await unitOfWork.Repository<IncomeEntity>()
            .UpdateAsync(x => x.Id == idDecoded && x.UserId.Equals(userId),
                prop =>
                    prop.SetProperty(c => c.Description, categoryModel.Description)
                        .SetProperty(c => c.CurrencyId, categoryModel.CurrencyId)
                        .SetProperty(c => c.CategoryId, categoryModel.CategoryId)
                        .SetProperty(c => c.Value, categoryModel.Value)
                        .SetProperty(c => c.ActualDateOfIncome, categoryModel.ActualDateOfIncome)
                        .SetProperty(c => c.UpdateDate, DateTime.UtcNow));

        return Results.NoContent();
    }
    
    private static async Task<IResult> DeleteIncome([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder, string id)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
        if (idDecoded == 0) return Results.BadRequest();

        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        await unitOfWork.Repository<IncomeEntity>()
            .DeleteAsync(x => x.Id == idDecoded && x.UserId.Equals(userId));

        return Results.NoContent();
    }

    public void DefineServices(IServiceCollection services)
    {
    }
}