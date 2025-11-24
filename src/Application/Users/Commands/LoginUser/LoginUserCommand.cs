using AutoMapper;
using FluentValidation;
using MediatR;
using OaigLoan.Application.Auth;
using OaigLoan.Application.Common.Interfaces;
using OaigLoan.Application.Users.Models;

namespace OaigLoan.Application.Users.Commands.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<AuthResult>;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<AuthResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            throw new ValidationException("Invalid credentials.");
        }

        var passwordValid = _passwordHasher.VerifyHashedPassword(user.PasswordHash, request.Password);
        if (!passwordValid)
        {
            throw new ValidationException("Invalid credentials.");
        }

        var jwt = _jwtTokenGenerator.GenerateToken(user);
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResult(jwt.AccessToken, jwt.ExpiresAtUtc, userDto);
    }
}
