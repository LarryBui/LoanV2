using OaigLoan.Application.Users.Models;

namespace OaigLoan.Application.Auth;

public sealed record AuthResult(string AccessToken, DateTime ExpiresAtUtc, UserDto User);
