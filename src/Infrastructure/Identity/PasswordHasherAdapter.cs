using Microsoft.AspNetCore.Identity;
using OaigLoan.Application.Common.Interfaces;

namespace OaigLoan.Infrastructure.Identity;

public class PasswordHasherAdapter : IPasswordHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();
    private readonly object _subject = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(_subject, password);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(_subject, hashedPassword, providedPassword);
        return result != PasswordVerificationResult.Failed;
    }
}
