using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        private ApplicationUser() { }

        private ApplicationUser(string email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            UserName = email;
            DateCreated = DateTime.UtcNow;
        }

      

        public DateTime DateCreated { get; private set; } = default!;


        public static ApplicationUser Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email cannot be null or empty.", nameof(email));
            }
            
            return new ApplicationUser(email);
        }

        public void UpdateEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
            {
                throw new ArgumentNullException("Email cannot be null or empty.", nameof(newEmail));
            }
            Email = newEmail;
            UserName = newEmail;
        }



    }
}
