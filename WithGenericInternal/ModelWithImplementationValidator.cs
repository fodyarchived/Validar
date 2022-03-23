using FluentValidation;

namespace WithGenericInternal;

public class ModelWithImplementationValidator :
    AbstractValidator<ModelWithImplementation>
{
    public ModelWithImplementationValidator()
    {
        RuleFor(x => x.Property1)
            .NotEmpty()
            .WithMessage("'Property1' message.");
        RuleFor(x => x.Property2)
            .NotEmpty()
            .WithMessage("'Property2' message.");
    }
}