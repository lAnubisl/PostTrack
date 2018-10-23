using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Models.EmailModels
{
    internal class PackageTrackingCancelledEmailModel : BaseEmailModel
    {
        internal readonly string Tracking;
        internal readonly string Description;

        internal PackageTrackingCancelledEmailModel(PackageDTO package) : base(package.Email)
        {
            Tracking = package.Tracking;
            Description = package.Description;
        }
    }
}