using FluentValidation;

namespace FilePocket.Contracts.Pocket.Validators;

public class CreateAndUpdatePocketValidator : AbstractValidator<CreateAndUpdatePocketRequest>
{
    public CreateAndUpdatePocketValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
