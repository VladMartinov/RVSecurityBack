using Core.Interfaces.Repositories;
using Exceptions.Exceptions.Users;

namespace Application.Extensions;

public static class UserRepositoryExtensions
{
    /// <summary>
    /// Does nothing if user exists, otherwise throws UserNotFoundException. 
    /// </summary>
    /// <param name="userRepository">User repository</param>
    /// <param name="id">User id</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="UserNotFound">Throws if user not found.</exception>
    public static async Task EnsureUserExists(this IUserRepository userRepository, Guid id, CancellationToken cancellationToken = default)
    {
        if (!await userRepository.UserExists(id, cancellationToken))
            throw new UserNotFound(id);
    }
}