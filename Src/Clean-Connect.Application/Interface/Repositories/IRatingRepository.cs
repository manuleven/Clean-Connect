using Clean_Connect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Interface.Repositories
{
    public interface IRatingRepository
    {
        Task<bool> AddRating(Ratings ratings, CancellationToken cancellationToken);
        Task<IEnumerable<Ratings>> GetRatingsByWorkerId(Guid workerId, CancellationToken cancellationToken);

        Task<bool> ExistAsync(Guid bookingId, CancellationToken cancellationToken);
    }
}
