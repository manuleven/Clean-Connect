using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Utilities
{

    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<RoleSeeder> _logger;

        public RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager, ILogger<RoleSeeder> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedRolesAsync()
        {
            var roles = new[] { "Admin", "Worker", "Client" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    _logger.LogInformation("Role {Role} created successfully", role);
                }
            }
        }
    }
}
