using Dorisoy.Pan.MediatR.Commands;
using FluentValidation;

namespace Dorisoy.Pan.MediatR.Validators
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(c => c.UserName).NotEmpty().WithMessage("UserName is required");
            RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}
