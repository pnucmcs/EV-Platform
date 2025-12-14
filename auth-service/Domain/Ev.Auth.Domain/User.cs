namespace Ev.Auth.Domain;

public sealed class User
{
    public Guid Id { get; init; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "user";
}
