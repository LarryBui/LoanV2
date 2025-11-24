using Microsoft.EntityFrameworkCore;
using OaigLoan.Application.Common.Interfaces;
using OaigLoan.Domain.Entities;

namespace OaigLoan.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly LoanDbContext _dbContext;

    public UserRepository(LoanDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }
}
