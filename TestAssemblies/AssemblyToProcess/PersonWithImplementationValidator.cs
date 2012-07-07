using FluentValidation;

public class PersonWithImplementationValidator : AbstractValidator<PersonWithImplementation>
{
    public PersonWithImplementationValidator()
    {
        RuleFor(x => x.FamilyName).NotEmpty();
        RuleFor(x => x.GivenNames).NotEmpty();
    }
}