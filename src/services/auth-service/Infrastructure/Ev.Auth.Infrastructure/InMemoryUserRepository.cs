using Ev.Auth.Domain;

namespace Ev.Auth.Infrastructure;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            _users.Add(user);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            _gate.Release();
        }
    }
}
