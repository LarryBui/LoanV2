using OaigLoan.Domain.Common;
using OaigLoan.Domain.Users.Events;

namespace OaigLoan.Domain.Entities;

public class User : Entity
{
    private User()
    {
    }

    private User(Guid id, string firstName, string lastName, string email, string phoneNumber, string passwordHash, DateTime createdAtUtc)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public static User Create(string firstName, string lastName, string email, string phoneNumber, string passwordHash)
    {
        var user = new User(Guid.NewGuid(), firstName.Trim(), lastName.Trim(), email.Trim().ToLowerInvariant(), phoneNumber.Trim(), passwordHash, DateTime.UtcNow);
        user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, user.Email));
        return user;
    }
}
