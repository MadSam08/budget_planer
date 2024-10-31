using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BudgetPlaner.UI.Extensions;

public class Razor
{
    public static RazorComponentResult Component<TComponent>(object? model = null) where TComponent : IComponent
    {
        return model is null
            ? new RazorComponentResult<TComponent>()
            : new RazorComponentResult<TComponent>(new { Model = model });
    }
}