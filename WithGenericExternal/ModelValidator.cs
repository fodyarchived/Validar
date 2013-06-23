using FluentValidation;

namespace WithGenericExternal
{
    public class ModelValidator : AbstractValidator<MyModel>
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
}