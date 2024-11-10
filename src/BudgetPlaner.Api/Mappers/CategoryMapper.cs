using BudgetPlaner.Domain;
using BudgetPlaner.Models.Api;
using Sqids;

namespace BudgetPlaner.Api.Mappers;

public static class CategoryMapper
{
    public static CategoryEntity MapToEntity(this CategoryModel model)
    {
        return new CategoryEntity
        {
            Name = model.Name,
            UserId = "",
            CategoryTypes = model.CategoryTypes,
        };
    }

    public static CategoryModel MapToModel(this CategoryEntity entity, SqidsEncoder<int> sqids)
    {
        return new CategoryModel
        {
            Id = sqids.Encode(entity.Id),
            Name = entity.Name,
            CategoryTypes = entity.CategoryTypes,
        };
    }

    public static IEnumerable<CategoryModel> MapToModel(this IEnumerable<CategoryEntity> entity,
        SqidsEncoder<int> sqids)
    {
        return entity.Select(x => new CategoryModel
        {
            Id = sqids.Encode(x.Id),
            Name = x.Name,
            CategoryTypes = x.CategoryTypes
        });
    }
}