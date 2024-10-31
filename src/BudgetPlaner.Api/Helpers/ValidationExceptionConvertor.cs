using BudgetPlaner.Models.Api;
using FluentValidation;
using FluentValidation.Results;

namespace BudgetPlaner.Api.Helpers;

public static class ValidationExceptionConvertor
{
    public static ApiErrorResponse GetApiErrorResponse(ValidationException exception,
        string traceIdentifier)
    {
        var response = new ApiErrorResponse
        {
            Code = nameof(ValidationException),
            InnerErrors = new ApiErrorResponse[exception.Errors.Count()],
            Message = exception.Message,
            Target = string.Empty,
            TraceIdentifier = traceIdentifier,
        };

        var index = 0;
        foreach (var validationFailure in exception.Errors)
        {
            response.InnerErrors[index] =
                ProcessValidationFailure(validationFailure, traceIdentifier);
            index++;
        }

        return response;
    }

    private static ApiErrorResponse ProcessValidationFailure(
        ValidationFailure validationFailure,
        string traceIdentifier)
    {

        var errorResponse = new ApiErrorResponse
        {
            InnerErrors = Array.Empty<ApiErrorResponse>(),
            Message = validationFailure.ErrorMessage,
            Target = validationFailure.PropertyName,
            TraceIdentifier = traceIdentifier,
        };

        return errorResponse;
    }
}

