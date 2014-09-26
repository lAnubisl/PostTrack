using System.Collections.Generic;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface IResponseReader
    {
        ICollection<PackageHistoryItemDTO> Read(string input);
    }
}