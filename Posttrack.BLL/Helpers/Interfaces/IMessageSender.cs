using System.Collections.Generic;
using System.Threading.Tasks;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface IMessageSender
    {
        Task SendStatusUpdateAsync(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update);

        Task SendInactivityEmailAsync(PackageDTO package);

        Task SendRegisteredAsync(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update);
    }
}