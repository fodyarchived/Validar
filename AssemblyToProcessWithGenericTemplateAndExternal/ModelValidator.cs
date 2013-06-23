using FluentValidation;

public class ModelValidator : AbstractValidator<Model2>
{
    public ModelValidator()
    {
        RuleFor(x => x.Property1)
            .NotEmpty()
            .WithMessage("'Property1' message.");
        RuleFor(x => x.Property2)
            .NotEmpty()
            .WithMessage("'Property2' message.");
    }
}