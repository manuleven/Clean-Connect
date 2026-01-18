using Clean_Connect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return await Task.FromResult(false);
            }

            var confimation = await _userManager.ConfirmEmailAsync(checkUser, request.Token);

            return await Task.FromResult(true);
        }
    }


}
