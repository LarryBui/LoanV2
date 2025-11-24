namespace OaigLoan.Bff.Contracts.Auth;

public sealed record RegisterRequest(string FirstName, string LastName, string Email, string PhoneNumber, string Password);
