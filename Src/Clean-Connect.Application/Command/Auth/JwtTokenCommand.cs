using Clean_Connect.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Clean_Connect.Application.Command.Auth
{
    public record JwtTokenCommand(ApplicationUser User) : IRequest<string>;

    public class JwtTokenCommandHandler : IRequestHandler<JwtTokenCommand, string>
    {
        private readonly IConfiguration _configuration;

        public JwtTokenCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> Handle(JwtTokenCommand request, CancellationToken cancellationToken)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, request.User.Id.ToString()),
                new Claim(ClaimTypes.Email, request.User.Email ),
                new Claim(ClaimTypes.Name, request.User.UserName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(jwtSettings["DurationInMinutes"])
                ),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(tokenString);
        }
    }
}
