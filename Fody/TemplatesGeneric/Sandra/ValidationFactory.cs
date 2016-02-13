using System;
using System.Collections.Concurrent;
using Sandra.SimpleValidator;
using Scalpel;

namespace GenericTemplates.Sandra
{

    [Remove]
    public static class ValidationFactory
    {
        static ConcurrentDictionary<RuntimeTypeHandle, IModelValidator> validators = new ConcurrentDictionary<RuntimeTypeHandle, IModelValidator>();

        public static ValidateThis<T> GetValidator<T>()
        {
            IModelValidator validator;
            var modelType = typeof(T);
            var typeHandle = modelType.TypeHandle;
            if (!validators.TryGetValue(typeHandle, out validator))
            {
                var validatorTypeName = $"{modelType.Namespace}.{modelType.Name}Validator";
                var validatorType = modelType.Assembly.GetType(validatorTypeName, true);
                validators[typeHandle] = validator = (IModelValidator)Activator.CreateInstance(validatorType);
            }
            return (ValidateThis<T>) validator;
        }
    }
}