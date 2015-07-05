using System.Collections.Generic;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface IEmailTemplateManager
    {
        string GetRegisteredEmailBody(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update);
        string GetInactivityEmailBody(PackageDTO package);
        string GetUpdateStatusEmailBody(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update);
    }
}