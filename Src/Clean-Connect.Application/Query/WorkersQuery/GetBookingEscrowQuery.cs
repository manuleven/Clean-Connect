using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using MediatR;

namespace Clean_Connect.Application.Query.WorkersQuery
{
    public record GetBookingEscrowQuery(Guid BookingId) : IRequest<EscrowDto?>;

    public class GetBookingEscrowQueryHandler : IRequestHandler<GetBookingEscrowQuery, EscrowDto?>
    {
        private readonly IUnitOfWork repo;

        public GetBookingEscrowQueryHandler(IUnitOfWork repo)
        {
            this.repo = repo;
        }

        public async Task<EscrowDto?> Handle(GetBookingEscrowQuery request, CancellationToken cancellationToken)
        {
            var escrow = await repo.Escrows.GetByBookingId(request.BookingId, cancellationToken);
            if (escrow == null)
                return null;

            return new EscrowDto(
                escrow.BookingId,
                escrow.PaymentId,
                escrow.WorkerId,
                escrow.Amount,
                escrow.Status,
                escrow.DateReleased);
        }
    }
}
