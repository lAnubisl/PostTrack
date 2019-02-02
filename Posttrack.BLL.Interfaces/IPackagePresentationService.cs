using System.Threading.Tasks;
using Posttrack.BLL.Interfaces.Models;

namespace Posttrack.BLL.Interfaces
{
    public interface IPackagePresentationService
    {
        Task Register(RegisterTrackingModel model);

        Task UpdateComingPackages();
    }
}