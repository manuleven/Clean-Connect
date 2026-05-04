using Clean_Connect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Helper
{
    public class WorkerWithDistance
    {
        public Worker Worker { get; set; }

        public double DistanceInKm { get; set; }
    }
}
