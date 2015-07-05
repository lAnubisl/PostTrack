using System.Collections.Generic;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.Data.Interfaces
{
    public interface IPackageDAO
    {
        ICollection<PackageDTO> LoadComingPackets();
        PackageDTO Load(string trackingNumber);
        void Register(RegisterPackageDTO package);
        bool Exists(string trackingNumber);
        void Update(PackageDTO package);
    }
}