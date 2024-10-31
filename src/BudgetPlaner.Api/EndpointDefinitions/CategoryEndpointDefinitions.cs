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

namespace BudgetPlaner.Api.EndpointDefinitions
{
    public class CategoryEndpointDefinitions : IEndpointDefinition
    {
        private const string BasePath = $"{EndpointNames.BudgetBasePath}/{EndpointNames.CategoryPath}";

        public void DefineEndpoints(WebApplication app)
        {
            app.MapGet(BasePath, GetCategories)
                .WithTags(SwaggerTags.CategoryTag)
                .RequireAuthorization();

            app.MapGet(BasePath + "/{id}", GetCategory)
                .WithTags(SwaggerTags.CategoryTag)
                .RequireAuthorization();

            app.MapPost(BasePath, AddCategory)
                .WithTags(SwaggerTags.CategoryTag)
                .RequireAuthorization();

            app.MapPut(BasePath + "/{id}", UpdateCategory)
                .WithTags(SwaggerTags.CategoryTag)
                .RequireAuthorization();

            app.MapDelete(BasePath + "/{id}", ArchiveCategory)
                .WithTags(SwaggerTags.CategoryTag)
                .RequireAuthorization();

            app.MapPut(BasePath + "/" + EndpointNames.RestorePath + "/{id}", RestoreCategory)
                .WithTags(SwaggerTags.CategoryTag)
                .RequireAuthorization();
        }

        private static async Task<IResult> GetCategories([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] SqidsEncoder<int> sqidsEncoder, CancellationToken cancellationToken = default)
        {
            var userId = httpContextAccessor.GetUserIdFromClaims();

            if (string.IsNullOrEmpty(userId))
                return Results.BadRequest();
            
            var response = await unitOfWork.Repository<CategoryEntity>()
                .Where(x => x.UserId.Equals(userId)).ToListAsync(cancellationToken: cancellationToken);

            return Results.Ok(response.MapToModel(sqidsEncoder));
        }

        private static async Task<IResult> GetCategory([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] SqidsEncoder<int> sqidsEncoder, string id)
        {
            var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();

            var userId = httpContextAccessor.GetUserIdFromClaims();
            if (string.IsNullOrEmpty(userId))
                return Results.BadRequest();
            
            var category = await unitOfWork.Repository<CategoryEntity>()
                .FirstOrDefaultAsync(x => x.Id == idDecoded && !x.IsDeleted && x.UserId.Equals(userId));

            return category == null ? Results.BadRequest() : Results.Ok(category.MapToModel(sqidsEncoder));
        }

        private static async Task<IResult> AddCategory([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromBody] CategoryModel categoryModel)
        {
            var entity = categoryModel.MapToEntity();
            var userId = httpContextAccessor.GetUserIdFromClaims();
            
            if (string.IsNullOrEmpty(userId))
                return Results.BadRequest();

            entity.UserId = userId;
            entity.CreateDate = DateTime.UtcNow;
            entity.UpdateDate = DateTime.UtcNow;
            
            await unitOfWork.Repository<CategoryEntity>().AddAsync(entity);
            await unitOfWork.Complete();
            return Results.NoContent();
        }

        private static async Task<IResult> UpdateCategory([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] SqidsEncoder<int> sqidsEncoder, string id, [FromBody] CategoryModel categoryModel)
        {
            var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
            if (idDecoded == 0) return Results.BadRequest();

            var userId = httpContextAccessor.GetUserIdFromClaims();
            
            if (string.IsNullOrEmpty(userId))
                return Results.BadRequest();
            
            await unitOfWork.Repository<CategoryEntity>()
                .UpdateAsync(x => x.Id == idDecoded && x.UserId.Equals(userId), 
                    prop =>
                    prop.SetProperty(c => c.Name, categoryModel.Name)
                        .SetProperty(c => c.CategoryTypes, categoryModel.CategoryTypes)
                        .SetProperty(c => c.UpdateDate, DateTime.UtcNow));

            return Results.NoContent();
        }

        private static async Task<IResult> ArchiveCategory([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] SqidsEncoder<int> sqidsEncoder, string id)
        {
            var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
            if (idDecoded == 0) return Results.BadRequest();
            
            var userId = httpContextAccessor.GetUserIdFromClaims();
            
            if (string.IsNullOrEmpty(userId))
                return Results.BadRequest();

            await unitOfWork.Repository<CategoryEntity>()
                .UpdateAsync(x => x.Id == idDecoded && x.UserId.Equals(userId), prop =>
                    prop.SetProperty(c => c.IsDeleted, true)
                        .SetProperty(c => c.UpdateDate, DateTime.UtcNow));

            return Results.NoContent();
        }

        private static async Task<IResult> RestoreCategory([FromServices] IUnitOfWork<BudgetPlanerContext> unitOfWork,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] SqidsEncoder<int> sqidsEncoder, string id)
        {
            var idDecoded = sqidsEncoder.Decode(id).SingleOrDefault();
            if (idDecoded == 0) return Results.BadRequest();
                
            var userId = httpContextAccessor.GetUserIdFromClaims();
            
            if (string.IsNullOrEmpty(userId))
                return Results.BadRequest();

            await unitOfWork.Repository<CategoryEntity>()
                .UpdateAsync(x => x.Id == idDecoded && x.UserId.Equals(userId), prop =>
                    prop.SetProperty(c => c.IsDeleted, false)
                        .SetProperty(c => c.UpdateDate, DateTime.UtcNow));

            return Results.NoContent();
        }

        public void DefineServices(IServiceCollection services)
        {
        }
    }
}