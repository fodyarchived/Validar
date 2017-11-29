using System;

namespace Validar
{
    /// <summary>
    /// Used to point to the correct validation Template.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ValidationTemplateAttribute : Attribute
    {
        /// <summary>
        /// Construct a new instance of <see cref="ValidationTemplateAttribute"/>
        /// </summary>
        /// <param name="validationTemplate">The validation template <see cref="Type"/> to use.</param>
        // ReSharper disable once UnusedParameter.Local
        public ValidationTemplateAttribute(Type validationTemplate)
        {
        }
    }
}