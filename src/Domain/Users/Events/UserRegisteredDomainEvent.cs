using OaigLoan.Domain.Common;

namespace OaigLoan.Domain.Users.Events;

public sealed record UserRegisteredDomainEvent(Guid UserId, string Email) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
