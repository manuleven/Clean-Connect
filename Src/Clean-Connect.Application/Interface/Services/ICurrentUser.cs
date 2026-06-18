using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Interface.Services
{
    public interface ICurrentUser
    {
        string? UserId { get; }

        string? Email { get; }

        string? Role { get; }

        bool IsAuthenticated { get; }
    }
}
