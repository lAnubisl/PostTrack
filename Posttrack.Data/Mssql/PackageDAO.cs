using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Newtonsoft.Json;
using Posttrack.Data.Interfaces;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.Data.Mssql
{
    public class PackageDAO : IPackageDAO
    {
        private const string LoadComingPackagesQuery = "select * from Package where IsFinished = 0";
        private const string LoadQuery = "select * from Package where Tracking = @trackingNumber";

        private const string RegisterQuery =
            "insert into Package (Tracking, Email, Description, CreateDate, UpdateDate, IsFinished) values (@Tracking, @Email, @Description, @CreateDate, @UpdateDate, 0)";

        private const string ExistsQuery =
            "select case when exists (select * from Package where Tracking = @trackingNumber) then 1 else 0 end";

        private const string UpdateQuery =
            "update Package set UpdateDate = @UpdateDate, IsFinished = @IsFinished, History = @History where Tracking = @Tracking";

        private readonly string connectionString;

        public PackageDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private SqlConnection NewConnection => new SqlConnection(connectionString);


        public ICollection<PackageDTO> LoadComingPackets()
        {
            using (var c = NewConnection)
            {
                return c.Query<Package>(LoadComingPackagesQuery).ToList().Select(x => x.Map()).ToList();
            }
        }

        public PackageDTO Load(string trackingNumber)
        {
            return Get(trackingNumber).Map();
        }

        public void Register(RegisterPackageDTO package)
        {
            using (var c = NewConnection)
            {
                c.Execute(RegisterQuery, package.Map());
            }
        }

        public bool Exists(string trackingNumber)
        {
            using (var c = NewConnection)
            {
                return c.QueryFirstOrDefault<bool>(ExistsQuery, new {trackingNumber});
            }
        }

        public void Update(PackageDTO package)
        {
            using (var c = NewConnection)
            {
                var entity = Get(package.Tracking);
                package.Map(entity);
                entity.UpdateDate = DateTime.Now;
                c.Execute(UpdateQuery, entity);
            }
        }

        /// <summary>
        /// This function was created to be used for mongodb->mssql data migration
        /// </summary>
        /// <param name="package"></param>
        public void Create(PackageDTO package)
        {
            using (var c = NewConnection)
            {
                c.Execute("insert into Package (Tracking, Email, Description, CreateDate, IsFinished, History) values (@Tracking, @Email, @Description, @CreateDate, @IsFinished, @History)", 
                    new Package
                    {
                        History = package.History == null ? null : JsonConvert.SerializeObject(package.History),
                        CreateDate = DateTime.Now,
                        Tracking = package.Tracking,
                        Description = package.Description,
                        Email = package.Email,
                        IsFinished = package.IsFinished,
                        UpdateDate = package.UpdateDate
                    });
            }
        }

        private Package Get(string trackingNumber)
        {
            using (var c = NewConnection)
            {
                return c.QueryFirstOrDefault<Package>(LoadQuery, new {trackingNumber});
            }
        }
    }
}