namespace OaigLoan.Application.Users.Models;

public sealed record UserDto(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber, DateTime CreatedAtUtc);
