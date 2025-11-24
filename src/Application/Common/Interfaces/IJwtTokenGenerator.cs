using OaigLoan.Application.Auth;
using OaigLoan.Domain.Entities;

namespace OaigLoan.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    JwtToken GenerateToken(User user);
}
