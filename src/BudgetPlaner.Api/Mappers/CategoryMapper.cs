using BudgetPlaner.Contracts.Api;
using BudgetPlaner.Contracts.Api.Category;
using BudgetPlaner.Domain;
using Sqids;

namespace BudgetPlaner.Api.Mappers;

public static class CategoryMapper
{
    public static CategoryEntity MapToEntity(this CategoryRequest request)
    {
        return new CategoryEntity
        {
            Name = request.Name,
            UserId = "",
            CategoryTypes = (int)request.CategoryTypes,
        };
    }

    public static CategoryRequest MapToModel(this CategoryEntity entity, SqidsEncoder<int> sqids)
    {
        return new CategoryRequest
        {
            Id = sqids.Encode(entity.Id),
            Name = entity.Name,
            CategoryTypes = (CategoryTypes)entity.CategoryTypes,
        };
    }

    public static IEnumerable<CategoryRequest> MapToModel(this IEnumerable<CategoryEntity> entity,
        SqidsEncoder<int> sqids)
    {
        return entity.Select(x => new CategoryRequest
        {
            Id = sqids.Encode(x.Id),
            Name = x.Name,
            CategoryTypes = (CategoryTypes)x.CategoryTypes
        });
    }
}