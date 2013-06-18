using System;

namespace Validar
{
    /// <summary>
    /// Used to flag items as requiring validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false)]
    public class InjectValidationAttribute : Attribute
    {
    }
}