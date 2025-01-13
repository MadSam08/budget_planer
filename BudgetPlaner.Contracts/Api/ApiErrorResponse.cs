using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace BudgetPlaner.Contracts.Api;


[ExcludeFromCodeCoverage]
public class ApiErrorResponse
{
    [JsonProperty(PropertyName = "code")]
    public string? Code { get; set; }

    [JsonProperty(PropertyName = "innerErrors")]
    public ApiErrorResponse[] InnerErrors { get; set; }

    [JsonProperty(PropertyName = "message")]
    public string? Message { get; set; }

    [JsonProperty(PropertyName = "stack")]
    public string? Stack { get; set; }

    [JsonProperty(PropertyName = "statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty(PropertyName = "target")]
    public string? Target { get; set; }

    [JsonProperty(PropertyName = "traceIdentifier")]
    public string? TraceIdentifier { get; set; }

    [JsonProperty(PropertyName = "customState")]
    public object? CustomState { get; set; }

    [JsonProperty(PropertyName = "details")]
    public Dictionary<string, string>? Details { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}