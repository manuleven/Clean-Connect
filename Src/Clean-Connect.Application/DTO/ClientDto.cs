using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.DTO
{
    public class ClientDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string Contact { get; set; }

        public Enum Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string State { get; set; }
    }
}
