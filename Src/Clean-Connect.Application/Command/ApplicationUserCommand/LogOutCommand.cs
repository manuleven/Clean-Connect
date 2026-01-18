using Clean_Connect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Clean_Connect.Application.Command.ApplicationUserCommand
{
    public record LogOutCommand() : IRequest<Unit>;

    public class LogOutCommandHandler: IRequestHandler<LogOutCommand, Unit>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        public LogOutCommandHandler(SignInManager<ApplicationUser> signInManager) =>
        
            _signInManager =signInManager;
        
        public async Task<Unit> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
            return Unit.Value;
        }
    }

}
