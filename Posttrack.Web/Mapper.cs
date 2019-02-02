using Posttrack.BLL.Interfaces.Models;
using Posttrack.Web.Models;

namespace Posttrack.Web
{
    internal static class Mapper
    {
        internal static RegisterTrackingModel Map(this SaveTrackingModel model)
        {
            return new RegisterTrackingModel
            {
                Tracking = model.Tracking,
                Email = model.Email,
                Description = model.Description
            };
        }
    }
}