using Posttrack.Web.Models;
using Posttrack.BLL.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Posttrack.Web
{
    internal static class Mapper
    {
        internal static RegisterTrackingModel Map(this SaveTrackingModel model)
        {
            var dto = new RegisterTrackingModel();
            dto.Tracking = model.Tracking;
            dto.Email = model.Email;
            dto.Description = model.Description;
            return dto;
        }
    }
}