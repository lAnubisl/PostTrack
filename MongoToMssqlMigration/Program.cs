using Posttrack.Data.Interfaces;
using Posttrack.Data.MongoDb;

namespace MongoToMssqlMigration
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IPackageDAO mongoDao = new PackageDAO("mongodb://localhost/tracking");
            var packages = mongoDao.LoadComingPackets();
            var mssqlDao =
                new Posttrack.Data.Mssql.PackageDAO("Data Source=.;Initial Catalog=tracking;Integrated Security=True");
            foreach (var package in packages)
            {
                mssqlDao.Create(package);
            }
        }
    }
}