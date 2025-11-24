using AutoMapper;
using FluentValidation;
using MediatR;
using OaigLoan.Application.Auth;
using OaigLoan.Application.Common.Interfaces;
using OaigLoan.Application.Users.Models;
using OaigLoan.Domain.Entities;

namespace OaigLoan.Application.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password) : IRequest<AuthResult>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IOutboxMessageRepository outboxMessageRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _outboxMessageRepository = outboxMessageRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AuthResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser is not null)
        {
            throw new ValidationException("Email already registered.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.FirstName, request.LastName, email, request.PhoneNumber, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        foreach (var domainEvent in user.DomainEvents)
        {
            var outboxMessage = OutboxMessage.FromDomainEvent(domainEvent);
            await _outboxMessageRepository.AddAsync(outboxMessage, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        user.ClearDomainEvents();

        var jwt = _jwtTokenGenerator.GenerateToken(user);
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResult(jwt.AccessToken, jwt.ExpiresAtUtc, userDto);
    }
}
