namespace OaigLoan.Api.Contracts.Auth;

public sealed record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, UserResponse User);
