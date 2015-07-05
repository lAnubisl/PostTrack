using System.Collections.Generic;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface IMessageSender
    {
        void SendStatusUpdate(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update);
        void SendInactivityEmail(PackageDTO package);
        void SendRegistered(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update);
    }
}