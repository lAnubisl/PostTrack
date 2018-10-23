using System.ComponentModel.DataAnnotations;
using Posttrack.BLL.Interfaces;

namespace Posttrack.Web
{
    public class UniqueTrackingNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validator = (IPackageValidator)validationContext.GetService(typeof(IPackageValidator));
            var validationResult = !validator.Exists((string)value);
            return validationResult ? ValidationResult.Success : new ValidationResult(this.ErrorMessage);
        }
    }
}