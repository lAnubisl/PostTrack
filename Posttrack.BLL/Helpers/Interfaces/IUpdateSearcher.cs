using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface IUpdateSearcher
    {
        string Search(PackageDTO paclage);
    }
}