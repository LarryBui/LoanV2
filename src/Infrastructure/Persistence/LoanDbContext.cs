using Microsoft.EntityFrameworkCore;
using OaigLoan.Application.Common.Interfaces;
using OaigLoan.Domain.Entities;

namespace OaigLoan.Infrastructure.Persistence;

public class LoanDbContext : DbContext, IUnitOfWork
{
    public LoanDbContext(DbContextOptions<LoanDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
