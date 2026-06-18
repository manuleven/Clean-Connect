using Microsoft.AspNetCore.Identity;

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
            IsClientProfileCompleted = false;
            IsWorkerProfileCompleted = false;

        }

      

        public DateTime DateCreated { get; private set; } = default!;
        public bool IsClientProfileCompleted { get; private set; } = default!;

        public bool IsWorkerProfileCompleted { get; private set; } = default!;

        public DateTime? EmailConfirmationSentAt { get; private set; } = default!;


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

        public void MarkEmailConfirmationSent()
        {
            EmailConfirmationSentAt = DateTime.UtcNow;
        }

        public void CompleteClientProfile() => IsClientProfileCompleted = true;
        public void CompleteWorkerProfile() => IsWorkerProfileCompleted = true;



    }
}
