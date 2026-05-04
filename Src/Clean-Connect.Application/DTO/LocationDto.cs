using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.DTO
{
    public record LocationDto
    {
        public int Latitude { get; set; }

        public int Longitude { get; set; }

        public string Address {  get; set; }

    }
}
