using Posttrack.BLL.Interfaces.Models;

namespace Posttrack.BLL.Interfaces
{
    public interface IPackagePresentationService
    {
        void Register(RegisterTrackingModel model);
        void UpdateComingPackages();
    }
}