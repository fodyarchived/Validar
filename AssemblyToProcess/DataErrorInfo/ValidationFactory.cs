using System;
using System.Collections.Concurrent;
using FluentValidation;

public static class ValidationFactory
{
    static ConcurrentDictionary<RuntimeTypeHandle, IValidator> validators = new ConcurrentDictionary<RuntimeTypeHandle, IValidator>();

    public static IValidator GetValidator(Type modelType)
    {
        IValidator validator;
        if (!validators.TryGetValue(modelType.TypeHandle, out validator))
        {
            var typeName = modelType.Name + "Validator";
            var type = Type.GetType(modelType.Namespace + "." + typeName, true);
            validators[modelType.TypeHandle] = validator = (IValidator)Activator.CreateInstance(type);
        }

        return validator;
    }
}