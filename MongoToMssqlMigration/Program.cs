using Posttrack.Data.Interfaces;
using Posttrack.Data.MongoDb;

namespace MongoToMssqlMigration
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MsSqlToMongo();
        }

        private static void MongoToMsSql()
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

        private static void MsSqlToMongo()
        {
            IPackageDAO sourceDao = new Posttrack.Data.Mssql.PackageDAO("Server=tcp:byalexblog.database.windows.net,1433;Initial Catalog=byalexblog;Persist Security Info=False;User ID=alex;Password=toortoor1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            var packages = sourceDao.LoadComingPackets();
            var targetDao = new PackageDAO("mongodb://dbposttrack2:Toortoor1!@mongo1.gear.host:27001/dbposttrack2");
            foreach (var package in packages)
            {
                targetDao.Save(package);
            }
        }
    }
}