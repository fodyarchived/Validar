using Sandra.SimpleValidator;
using Sandra.SimpleValidator.Rules;
using Scalpel;

namespace TemplatesGeneric.Sandra
{
    [Remove]
    public class ModelValidator : 
        ValidateThis<Model>
    {
        public ModelValidator()
        {
            For(x => x.Property1)
                .Ensure(new Required().WithMessage("'Property1' message."));

            For(x => x.Property2)
                .Ensure(new Required().WithMessage("'Property2' message."));
        }
    }
}