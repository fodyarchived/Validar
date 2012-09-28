using FluentValidation;

public class PersonWithStaticConstructorValidator : AbstractValidator<PersonWithStaticConstructor>
{
    public PersonWithStaticConstructorValidator()
    {
        RuleFor(x => x.FamilyName).NotEmpty();
        RuleFor(x => x.GivenNames).NotEmpty();
    }
}