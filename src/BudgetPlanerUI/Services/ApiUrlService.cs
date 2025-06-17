namespace BudgetPlaner.UI.Services;

public interface IApiUrlService
{
    string GetApiUrl(string path);
}

public class ApiUrlService : IApiUrlService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ApiUrlService> _logger;

    public ApiUrlService(IHttpContextAccessor httpContextAccessor, ILogger<ApiUrlService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string GetApiUrl(string path)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext != null)
        {
            // Use the current request's scheme and host to construct API URLs
            var request = httpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var apiUrl = $"{baseUrl}/api/{path.TrimStart('/')}";
            
            _logger.LogDebug("Constructed API URL: {ApiUrl} from base: {BaseUrl}", apiUrl, baseUrl);
            return apiUrl;
        }
        
        // Fallback for cases where HttpContext is not available
        var fallbackUrl = $"http://localhost:5271/api/{path.TrimStart('/')}";
        _logger.LogWarning("HttpContext not available, using fallback URL: {FallbackUrl}", fallbackUrl);
        return fallbackUrl;
    }
} 