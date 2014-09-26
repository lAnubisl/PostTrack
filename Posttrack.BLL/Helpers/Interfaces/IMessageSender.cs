using System.Collections.Generic;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface IMessageSender
    {
        bool SendStatusUpdate(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update);

        void SendInactivityEmail(PackageDTO package);

        void SendRegistered(RegisterPackageDTO package);
    }
}