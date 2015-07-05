using System.ComponentModel.DataAnnotations;
using Posttrack.BLL.Interfaces;
using Posttrack.DI;

namespace Posttrack.Web
{
    public class UniqueTrackingNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var validator = InversionOfControlContainer.Instance.Resolve<IPackageValidator>();
            return !validator.Exists((string) value);
        }
    }
}