using Dorisoy.Pan.MediatR.Commands;
using FluentValidation;

namespace Dorisoy.Pan.MediatR.Validators
{
    public class AddFolderCommandValidator : AbstractValidator<AddFolderCommand>
    {
        public AddFolderCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
