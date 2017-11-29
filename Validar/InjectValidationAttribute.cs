using System;

namespace Validar
{
    /// <summary>
    /// Used to flag items as requiring validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectValidationAttribute : Attribute
    {
    }
}