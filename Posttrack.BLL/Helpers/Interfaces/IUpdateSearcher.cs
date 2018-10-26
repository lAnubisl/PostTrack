using Posttrack.Data.Interfaces.DTO;
using System.Threading.Tasks;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface IUpdateSearcher
    {
        Task<string> SearchAsync(PackageDTO paclage);
    }
}