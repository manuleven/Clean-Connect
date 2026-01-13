using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        private ApplicationUser() { }

        private ApplicationUser(string userName, string email, string password)
        {
            Email = email;
        }

    }
}
