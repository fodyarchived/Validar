using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using FluentValidation;

namespace Generic
{
    public static class ValidationFactory
    {
        static ConcurrentDictionary<RuntimeTypeHandle, IValidator> validators = new ConcurrentDictionary<RuntimeTypeHandle, IValidator>();

        public static IValidator<T> GetValidator<T>()
            where T : INotifyPropertyChanged
        {            var modelType = typeof (T);
            var modelTypeHandle = modelType.TypeHandle;
            if (!validators.TryGetValue(modelTypeHandle, out var validator))
            {
                var typeName = $"{modelType.Namespace}.{modelType.Name}Validator";
                var type = modelType.Assembly.GetType(typeName, true);
                validators[modelTypeHandle] = validator = (IValidator) Activator.CreateInstance(type);
            }

            return (IValidator<T>) validator;
        }
    }
}