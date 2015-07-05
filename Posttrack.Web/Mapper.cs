using Posttrack.BLL.Interfaces.Models;
using Posttrack.Web.Models;

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