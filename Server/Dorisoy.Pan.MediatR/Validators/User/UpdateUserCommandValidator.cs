using Dorisoy.Pan.MediatR.Commands;
using FluentValidation;

namespace Dorisoy.Pan.MediatR.Validators
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(c => c.RaleName).NotEmpty().WithMessage("RaleName is Required");
        }
    }
}
