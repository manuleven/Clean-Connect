using Clean_Connect.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Entities
{
    public class Ratings : BaseEntity
    {
        private Ratings() { }

        private Ratings(Guid workerId, Guid clientId, Guid bookingId, int ratingValue, string? comment, string? createdBy = null)
        {
            WorkerId = workerId;
            ClientId = clientId;

           
            BookingId = bookingId;
            RatingValue = ratingValue;
            Comment = comment;
            UpdateMetadata(createdBy);
        }

        public Guid WorkerId { get; set; } = default!;

        public int RatingValue { get; set; } = default!;
       
        public string? Comment { get; set; } = default!;

        public Worker Worker { get; set; } = default!;

        public Guid BookingId { get; set; } = default!;
        public Guid ClientId { get; set; } = default!;

        public static Ratings Create(Guid workerId, Guid clientId, Guid bookingId, int ratingValue, string? comment, string? createdBy = null)
        {
            if (ratingValue < 1 || ratingValue > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(RatingValue), "Rating value must be between 1 and 5.");
            }
            var ratings = new Ratings(workerId, clientId, bookingId,  ratingValue, comment, createdBy);
            return ratings;
        }

      

    }
}
