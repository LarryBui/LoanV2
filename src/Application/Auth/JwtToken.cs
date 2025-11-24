namespace OaigLoan.Application.Auth;

public sealed record JwtToken(string AccessToken, DateTime ExpiresAtUtc);
