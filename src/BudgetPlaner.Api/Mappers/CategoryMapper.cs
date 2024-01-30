using BudgetPlaner.Models.ApiResponse;
using BudgetPlaner.Models.Domain;
using Sqids;

namespace BudgetPlaner.Api.Mappers;

public static class CategoryMapper
{
    public static IEnumerable<CategoryEntity> MapToEntity(this IEnumerable<CategoryModel> model, SqidsEncoder<int> sqids)
    {
        return model.Select(x =>
        {
            var single = sqids.Decode(x.Id).SingleOrDefault();
            return new CategoryEntity
            {
                Id = single,
                Name = x.Name,
                UserId = "",
                CategoryTypes = x.CategoryTypes
            };
        });
    }
    
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