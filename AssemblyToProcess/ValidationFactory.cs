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
            var typeName = string.Format("{0}.{1}Validator", modelType.Namespace, modelType.Name);
            var type = Type.GetType(typeName, true);
            validators[modelType.TypeHandle] = validator = (IValidator)Activator.CreateInstance(type);
        }

        return validator;
    }
}