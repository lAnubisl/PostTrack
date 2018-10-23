using Posttrack.BLL.Interfaces.Models;
using System.Threading.Tasks;

namespace Posttrack.BLL.Interfaces
{
    public interface IPackagePresentationService
    {
        Task Register(RegisterTrackingModel model);
        Task UpdateComingPackages();
    }
}