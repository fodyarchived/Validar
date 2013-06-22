using Sandra.SimpleValidator;
using Sandra.SimpleValidator.Rules;

public class PersonValidator : ValidateThis<Person>
{
    public PersonValidator()
    {
        For(x => x.GivenNames)
            .Ensure(new Required().WithMessage("'Given Names' should not be empty."));

        For(x => x.FamilyName)
            .Ensure(new Required().WithMessage("'Family Name' should not be empty."));
    }
}