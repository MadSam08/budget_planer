using FluentValidation;

namespace BudgetPlaner.Api.Helpers;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argToValidate = context.Arguments.FirstOrDefault(x => x is T) as T;

        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is null) return await next.Invoke(context);
        var validationResult = await validator.ValidateAsync(argToValidate!);
        if (validationResult.IsValid) return await next.Invoke(context);
        var traceIdentifier = context.HttpContext.TraceIdentifier;

        var errorsResponse = ValidationExceptionConvertor.GetApiErrorResponse(new ValidationException(validationResult.Errors), traceIdentifier);

        return Results.BadRequest(errorsResponse);
    }
}