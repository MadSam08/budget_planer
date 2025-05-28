using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Constants.EndpointNames;
using BudgetPlaner.Api.Extensions;
using BudgetPlaner.Api.Mappers;
using BudgetPlaner.Application.Services.Credit;
using BudgetPlaner.Contracts.Api.Loan;
using BudgetPlaner.Domain;
using BudgetPlaner.Infrastructure.DatabaseContext;
using BudgetPlaner.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sqids;

namespace BudgetPlaner.Api.EndpointDefinitions;

public class LoanEndpointDefinitions : IEndpointDefinition
{
    private const string BasePath = $"{EndpointNames.BudgetBasePath}/{EndpointNames.LoanPath}";
    
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet(BasePath, GetCredits)
            .WithTags(SwaggerTags.LoanTag)
            .RequireAuthorization();
        
        app.MapGet(BasePath + "/{id}", GetCredit)
            .WithTags(SwaggerTags.LoanTag)
            .RequireAuthorization();

        app.MapPost(BasePath, AddCredit)
            .WithTags(SwaggerTags.LoanTag)
            .RequireAuthorization();
        
        app.MapPost(BasePath+ "/{id}", GenerateInterestRate)
            .WithTags(SwaggerTags.LoanTag)
            .RequireAuthorization();

        app.MapPut(BasePath + "/{id}", UpdateCredit)
            .WithTags(SwaggerTags.LoanTag)
            .RequireAuthorization();
    }

    private static async Task<IResult> GenerateInterestRate([FromServices] ILoanService loanService,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder, string id)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
        
        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        await loanService.GenerateCreditInterestRates(idDecoded, userId);

        return Results.NoContent();
    }

    private static async Task<IResult> GetCredits([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();

        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();
            
        var response = await unitOfWork.Repository<LoanEntity>()
            .Where(x => x.UserId.Equals(userId)).ToListAsync();

        return Results.Ok(response.MapToModel(sqidsEncoder));
    }
    
    private static async Task<IResult> GetCredit([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder, string id)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();

        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();
            
        var creditEntity = await unitOfWork.Repository<LoanEntity>()
            .FirstOrDefaultAsync(x => x.Id == idDecoded && x.UserId.Equals(userId));

        return creditEntity == null ? Results.BadRequest() : Results.Ok(creditEntity.MapToModel(sqidsEncoder));
    }
    
    private static async Task<IResult> AddCredit([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromBody] LoanModel loanModel)
    {
        var userId = httpContextAccessor.GetUserIdFromClaims();
        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();

        var entity = loanModel.MapToEntity();
        entity.UserId = userId;
        entity.CreateDate = DateTime.UtcNow;
        entity.UpdateDate = DateTime.UtcNow;
            
        await unitOfWork.Repository<LoanEntity>().AddAsync(entity);
        await unitOfWork.Complete();
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateCredit([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] SqidsEncoder<int> sqidsEncoder, string id, [FromBody] LoanModel loanModel)
    {
        var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
        if (idDecoded == 0) return Results.BadRequest();

        var userId = httpContextAccessor.GetUserIdFromClaims();
            
        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest();
            
        await unitOfWork.Repository<LoanEntity>()
            .UpdateAsync(x => x.Id == idDecoded && x.UserId.Equals(userId), 
                prop =>
                    prop.SetProperty(c => c.Name, loanModel.Name)
                        .SetProperty(c => c.Interest, loanModel.Interest)
                        .SetProperty(c => c.CurrencyId, loanModel.CurrencyId)
                        .SetProperty(c => c.Period, loanModel.Period)
                        .SetProperty(c => c.BankName, loanModel.BankName)
                        .SetProperty(c => c.TotalAmount, loanModel.TotalValue)
                        .SetProperty(c => c.AnnualRate, loanModel.AnnualRate)
                        .SetProperty(c => c.APR, loanModel.APR)
                        .SetProperty(c => c.UpdateDate, DateTime.UtcNow));

        return Results.NoContent();
    }

    public void DefineServices(IServiceCollection services)
    {
    }
}