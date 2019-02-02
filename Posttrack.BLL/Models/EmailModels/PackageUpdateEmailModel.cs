using System.Collections.Generic;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Models.EmailModels
{
    internal class PackageUpdateEmailModel : BaseEmailModel
    {
        internal readonly string Tracking;
        internal readonly string Description;
        internal readonly string Update;

        internal PackageUpdateEmailModel(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
            : base(package.Email)
        {
            Tracking = package.Tracking;
            Description = package.Description;
            Update = LoadHistoryTemplate(package.History, update);
        }
    }
}