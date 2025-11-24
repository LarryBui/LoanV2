using OaigLoan.Domain.Entities;

namespace OaigLoan.Application.Common.Interfaces;

public interface IOutboxMessageRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken);
}
