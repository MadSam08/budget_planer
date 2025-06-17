using BudgetPlaner.Contracts.Api.Profile;
using Microsoft.Extensions.Logging;

namespace BudgetPlaner.UI.ApiClients;

public class UserProfileService : BaseAuthenticatedService, IUserProfileService
{
    private const string BasePath = "budget-planer/profile";

    public UserProfileService(HttpClient client, ILogger<UserProfileService> logger)
        : base(client, logger)
    {
    }

    public async Task<UserProfileModel?> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<UserProfileModel>(BasePath, cancellationToken);
    }

    public async Task<bool> UpdateProfileAsync(UserProfileModel profile, CancellationToken cancellationToken = default)
    {
        return await PutAsync(BasePath, profile, cancellationToken);
    }
}
