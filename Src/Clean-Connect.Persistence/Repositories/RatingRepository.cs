using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Persistence.Repositories
{
    public class RatingRepository(ApplicationDbContext dbContext) : IRatingRepository
    {
        public async Task<bool> AddRating(Ratings ratings, CancellationToken cancellationToken)
        {
            await dbContext.Ratings.AddAsync(ratings, cancellationToken);
            return true;
        }

        public async Task<IEnumerable<Ratings>> GetRatingsByWorkerId(Guid workerId, CancellationToken cancellationToken)
        {
            return await dbContext.Ratings.Where(r => r.WorkerId == workerId).ToListAsync();
        }

        public async Task<bool>  ExistAsync(Guid bookingId, CancellationToken cancellationToken)
        {
           return await dbContext.Ratings.AnyAsync(x => x.BookingId == bookingId);
        }
    }
}
