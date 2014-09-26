using Posttrack.BLL.Interfaces;
using Posttrack.DI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Posttrack.Web
{
    public class UniqueTrackingNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var validator = InversionOfControlContainer.Instance.Resolve<IPackageValidator>();
            return !validator.Exists((string)value);
        }
    }
}