using System.Collections.Generic;
using System.Threading.Tasks;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface IMessageSender
    {
        Task SendStatusUpdate(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update);
        Task SendInactivityEmail(PackageDTO package);
        Task SendRegistered(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update);
    }
}