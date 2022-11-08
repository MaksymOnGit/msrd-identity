using FluentResults;
using MSRD.Identity.Core.Auth.Models;
using MSRD.Identity.Core.Query;

namespace MSRD.Identity.PersistentStorage.Repositories.Interfaces
{
    public interface IIdentityRepository
    {
        ValueTask<Result> ConfirmEmailAsync(User user, string token, CancellationToken cancellationToken = default);
        ValueTask<Result> CrateEmptyUserAsync(string email, CancellationToken cancellationToken = default);
        ValueTask<User> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        ValueTask<string> GenerateEmailConfirmationTokenAsync(User user, CancellationToken cancellationToken = default);
        ValueTask<QueryResponse<UserInfoView>> QueryUsersNoTrackingAsync(QueryRequest queryRequest, CancellationToken cancellationToken = default);
        ValueTask<bool> IsEmailOccupiedAsync(string email, CancellationToken cancellationToken = default);
        ValueTask SaveChangesAsync(CancellationToken cancellationToken = default);
        Task SeedUsersAndRoles();
        ValueTask<Result> SetUserPasswordAsync(User user, string password, CancellationToken cancellationToken = default);
    }
}