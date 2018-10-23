using System.Collections.Generic;
using System.Threading.Tasks;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.Data.Interfaces
{
    public interface IPackageDAO
    {
        Task<ICollection<PackageDTO>> LoadTrackingAsync();
        Task<PackageDTO> LoadAsync(string trackingNumber);
        Task RegisterAsync(RegisterPackageDTO package);
        bool Exists(string trackingNumber);
        Task UpdateAsync(PackageDTO package);
    }
}