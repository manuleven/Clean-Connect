using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using MediatR;

namespace Clean_Connect.Application.Query.WorkersQuery
{
    public record GetWorkerWalletQuery(Guid WorkerId) : IRequest<WalletDto>;

    public class GetWorkerWalletQueryHandler : IRequestHandler<GetWorkerWalletQuery, WalletDto>
    {
        private readonly IUnitOfWork repo;

        public GetWorkerWalletQueryHandler(IUnitOfWork repo)
        {
            this.repo = repo;
        }

        public async Task<WalletDto> Handle(GetWorkerWalletQuery request, CancellationToken cancellationToken)
        {
            var wallet = await repo.Wallets.GetByWorkerId(request.WorkerId, cancellationToken);
            if (wallet == null)
                return new WalletDto(request.WorkerId, 0, 0);

            return new WalletDto(wallet.WorkerId, wallet.Balance, wallet.TotalEarned);
        }
    }
}
