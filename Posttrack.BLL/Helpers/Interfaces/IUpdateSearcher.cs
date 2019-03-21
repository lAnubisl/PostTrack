using System.Threading.Tasks;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface IUpdateSearcher
    {
        Task<string> SearchAsync(PackageDTO package);
    }
}