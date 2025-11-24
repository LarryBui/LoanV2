using OaigLoan.Application.Common.Interfaces;
using OaigLoan.Domain.Entities;

namespace OaigLoan.Infrastructure.Persistence.Repositories;

public class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly LoanDbContext _dbContext;

    public OutboxMessageRepository(LoanDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
    }
}
