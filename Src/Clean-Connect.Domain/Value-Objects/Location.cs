using Clean_Connect.Domain.Utilities;
using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Value_Objects
{
    //public class LocationDtoValidator : AbstractValidator<LocationDto>
    //{
    //    public LocationDtoValidator()
    //    {
    //        RuleFor(x => x.Latitude)
    //            .InclusiveBetween(-90, 90)
    //            .WithMessage("Latitude must be between -90 and 90.");

    //        RuleFor(x => x.Longitude)
    //            .InclusiveBetween(-180, 180)
    //            .WithMessage("Longitude must be between -180 and 180.");

    //        RuleFor(x => x.Address)
    //            .NotEmpty().WithMessage("Address is required.")
    //            .MinimumLength(5)
    //            .MaximumLength(200);
    //    }
    //}
    public class Location : ValueObjects
    {

        public double Latitude { get; }
        public double Longitude { get; }
        public Point Point { get; }
        

        private Location() { }

        private Location(double  latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;

                Point = new Point(longitude, latitude) { SRID = 4326 };

        }

        public static Location Create(double latitude, double longitude)
        {
            if (latitude < 4.0 || latitude > 14.0)
                throw new ArgumentOutOfRangeException("Latitude must be between 4.0 and 14.0 .");

            if (longitude < 2.5 || longitude > 15.5)
                throw new ArgumentOutOfRangeException("longitude must be between 2.5 and 15.5 .");


            return new Location(latitude, longitude);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }

    }
}
