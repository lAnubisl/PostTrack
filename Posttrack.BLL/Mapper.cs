using Posttrack.BLL.Interfaces.Models;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL
{
    internal static class Mapper
    {
        internal static RegisterPackageDTO Map(this RegisterTrackingModel model)
        {
            var dto = new RegisterPackageDTO();
            dto.Description = model.Description;
            dto.Email = model.Email;
            dto.Tracking = model.Tracking;
            return dto;
        }
    }
}