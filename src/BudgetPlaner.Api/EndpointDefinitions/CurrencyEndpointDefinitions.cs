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

public class CurrencyEndpointDefinitions : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet(ApiEndpoints.Currencies.GetAll, GetCurrencies)
            .WithTags(SwaggerTags.CurrencyTag)
            .RequireAuthorization();

        app.MapGet(ApiEndpoints.Currencies.Get, GetCurrency)
            .WithTags(SwaggerTags.CurrencyTag)
            .RequireAuthorization();

        app.MapPost(ApiEndpoints.Currencies.Create, AddCurrency)
            .WithTags(SwaggerTags.CurrencyTag)
            .RequireAuthorization();

        app.MapPut(ApiEndpoints.Currencies.Update, UpdateCurrency)
            .WithTags(SwaggerTags.CurrencyTag)
            .RequireAuthorization();

        app.MapDelete(ApiEndpoints.Currencies.Delete, ArchiveCurrency)
            .WithTags(SwaggerTags.CurrencyTag)
            .RequireAuthorization();

        app.MapPatch(ApiEndpoints.Currencies.Restore, RestoreCurrency)
            .WithTags(SwaggerTags.CurrencyTag)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetCurrencies([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        var response = await unitOfWork.Repository<CurrencyEntity>()
            .Where(x => x.UserId.Equals(userId) && !x.IsDeleted).ToListAsync();

        return Results.Ok(response.MapToModel(sqidsEncoder));
    }

    private static async Task<IResult> GetCurrency([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder,
        string id)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();

        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();
            
        var currencyEntity = await unitOfWork.Repository<CurrencyEntity>()
            .FirstOrDefaultAsync(x => x.Id == idDecoded && !x.IsDeleted && x.UserId.Equals(userId));

        return currencyEntity == null ? Results.BadRequest() : Results.Ok(currencyEntity.MapToModel(sqidsEncoder));
    }

    private static async Task<IResult> AddCurrency([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromBody] CurrencyModel currencyModel)
    {
        var entity = currencyModel.MapToEntity();
        var userId = httpContextAccessor.GetUserIdFromClaims();
            
        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        entity.UserId = userId;
        entity.CreateDate = DateTime.UtcNow;
        entity.UpdateDate = DateTime.UtcNow;
            
        await unitOfWork.Repository<CurrencyEntity>().AddAsync(entity);
        await unitOfWork.Complete();
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateCurrency([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder,
        string id,
        [FromBody] CurrencyModel currencyModel)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
        if (idDecoded == 0) return Results.BadRequest();

        var userId = httpContextAccessor.GetUserIdFromClaims();
            
        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();
            
        await unitOfWork.Repository<CurrencyEntity>()
            .UpdateAsync(x => x.Id == idDecoded && x.UserId.Equals(userId), 
                prop =>
                    prop.SetProperty(c => c.Name, currencyModel.Name)
                        .SetProperty(c => c.Code, currencyModel.Code)
                        .SetProperty(c => c.NationalBankRate, currencyModel.NationalBankRate)
                        .SetProperty(c => c.UpdateDate, DateTime.UtcNow));

        return Results.NoContent();
    }

    private static async Task<IResult> ArchiveCurrency([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder,
        string id)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
        if (idDecoded == 0) return Results.BadRequest();
            
        var userId = httpContextAccessor.GetUserIdFromClaims();
            
        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        await unitOfWork.Repository<CurrencyEntity>()
            .UpdateAsync(x => x.Id == idDecoded && x.UserId.Equals(userId), prop =>
                prop.SetProperty(c => c.IsDeleted, true)
                    .SetProperty(c => c.UpdateDate, DateTime.UtcNow));

        return Results.NoContent();
    }

    private static async Task<IResult> RestoreCurrency([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder,
        string id)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
        if (idDecoded == 0) return Results.BadRequest();
            
        var userId = httpContextAccessor.GetUserIdFromClaims();
            
        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        await unitOfWork.Repository<CurrencyEntity>()
            .UpdateAsync(x => x.Id == idDecoded && x.UserId.Equals(userId), prop =>
                prop.SetProperty(c => c.IsDeleted, false)
                    .SetProperty(c => c.UpdateDate, DateTime.UtcNow));

        return Results.NoContent();
    }

    public void DefineServices(IServiceCollection services)
    {
    }
}