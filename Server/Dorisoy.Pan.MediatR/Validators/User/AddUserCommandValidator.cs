using Dorisoy.Pan.MediatR.Commands;
using FluentValidation;

namespace Dorisoy.Pan.MediatR.Validators
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            RuleFor(c => c.UserName).NotEmpty().WithMessage("Please enter username.");
            RuleFor(c => c.RaleName).NotEmpty().WithMessage("Please enter RaleName.");
            RuleFor(c => c.Email).NotEmpty().WithMessage("Please enter email .");
            RuleFor(c => c.Email).EmailAddress().WithMessage("Email in right format.");
            RuleFor(c => c.Password).NotEmpty().WithMessage("Please enter password.");
        }
    }
}
