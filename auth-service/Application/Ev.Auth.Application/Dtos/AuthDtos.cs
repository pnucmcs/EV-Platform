namespace Ev.Auth.Application.Dtos;

public sealed record RegisterRequest(string Email, string Password, string Role = "user");
public sealed record LoginRequest(string Email, string Password);
public sealed record AuthResponse(Guid UserId, string Email, string Role, string Token);
