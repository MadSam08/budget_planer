using BudgetPlaner.Contracts.Api.Profile;

namespace BudgetPlaner.UI.ApiClients;

public interface IUserProfileService
{
    Task<UserProfileModel?> GetProfileAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateProfileAsync(UserProfileModel profile, CancellationToken cancellationToken = default);
}
