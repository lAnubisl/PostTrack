using Posttrack.BLL.Interfaces;
using Posttrack.DI;

namespace Posttrack.Checker
{
    public class Checker
    {
        public static void Main()
        {
            var service = InversionOfControlContainer.Instance.Resolve<IPackagePresentationService>();
            service.UpdateComingPackages();
        }
    }
}