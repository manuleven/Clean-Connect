using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.DTO
{
    public record NearbyWorkerDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string Contact { get; set; }

        public string Gender { get; set; }

        public string ServiceType { get; set; }

        public double DistanceInKm { get; set; }

        public double Rating { get; set; }
    }
}
