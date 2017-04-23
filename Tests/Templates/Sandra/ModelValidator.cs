using Sandra.SimpleValidator;
using Sandra.SimpleValidator.Rules;

namespace Templates.Sandra
{
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