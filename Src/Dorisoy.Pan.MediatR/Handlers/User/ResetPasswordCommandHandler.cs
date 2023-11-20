using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Helper;
using Microsoft.Extensions.Logging;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ServiceResponse<UserDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;
        public ResetPasswordCommandHandler(
            UserManager<User> userManager,
            ILogger<ResetPasswordCommandHandler> logger
            )
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserDto>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var entity = await _userManager.FindByEmailAsync(request.UserName);
            if (entity == null)
            {
                _logger.LogError("User not Found.");
                return ServiceResponse<UserDto>.ReturnFailed(404, "User not Found.");
            }
            string code = await _userManager.GeneratePasswordResetTokenAsync(entity);
            IdentityResult passwordResult = await _userManager.ResetPasswordAsync(entity, code, request.Password);
            if (!passwordResult.Succeeded)
            {
                return ServiceResponse<UserDto>.Return500();
            }
            return ServiceResponse<UserDto>.ReturnSuccess();
        }
    }
}
