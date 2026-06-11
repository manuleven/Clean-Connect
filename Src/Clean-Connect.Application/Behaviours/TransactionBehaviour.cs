using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace Clean_Connect.Application.Behaviours
{
    public class TransactionBehaviour<TRequest, TResponse>
          : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ApplicationDbContext _dbContext;

        public TransactionBehaviour(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // 🔒 If already in transaction, just continue (prevents nesting issues)
            if (_dbContext.Database.CurrentTransaction != null)
            {
                return await next();
            }

            await using IDbContextTransaction transaction =
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return response;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}