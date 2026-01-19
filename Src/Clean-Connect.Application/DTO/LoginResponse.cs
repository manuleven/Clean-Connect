using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.DTO
{
    public record LoginResponse
    {
        public Guid UserId { get; set; }

        public string Token { get; set; }
    }
}
