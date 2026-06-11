using Clean_Connect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Clean_Connect.Application.Command.ApplicationUserCommand
{
    public record ConfirmEmailCommand(string UserId, string Token) : IRequest<bool>;

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var checkUser = await _userManager.FindByIdAsync(request.UserId);
            if (checkUser == null)
            {
                return false;
            }

            var confimation = await _userManager.ConfirmEmailAsync(checkUser, request.Token);

            return confimation.Succeeded;
        }
    }


}
