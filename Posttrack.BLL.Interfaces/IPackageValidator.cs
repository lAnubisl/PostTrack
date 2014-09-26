namespace Posttrack.BLL.Interfaces
{
    public interface IPackageValidator
    {
        bool Exists(string trackingNumber);
    }
}