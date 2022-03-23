﻿using FluentValidation;

namespace TemplatesGeneric.FluentValidation;

public class ModelValidator :
    AbstractValidator<Model>
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