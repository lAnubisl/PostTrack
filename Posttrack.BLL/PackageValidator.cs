using Posttrack.BLL.Interfaces;
using Posttrack.Data.Interfaces;

namespace Posttrack.BLL
{
    public class PackageValidator : IPackageValidator
    {
        private readonly IPackageDAO packageDAO;

        public PackageValidator(IPackageDAO packageDAO)
        {
            this.packageDAO = packageDAO;
        }

        public bool Exists(string trackingNumber)
        {
            return packageDAO.Exists(trackingNumber);
        }
    }
}