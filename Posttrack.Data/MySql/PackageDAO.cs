using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using Posttrack.Data.Interfaces;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.Data.MySql
{
    public class PackageDAO : BaseDAO, IPackageDAO
    {
        private const string LoadComingPackagesQuery = "select * from Package where IsFinished = 0";
        private const string LoadQuery = "select * from Package where Tracking = @trackingNumber";

        private const string RegisterQuery =
            "insert into Package (Tracking, Email, Description, CreateDate, UpdateDate, IsFinished) values (@Tracking, @Email, @Description, @CreateDate, @UpdateDate, 0)";

        private const string ExistsQuery =
            "select case when exists (select * from Package where Tracking = @trackingNumber) then 1 else 0 end";

        private const string UpdateQuery =
            "update Package set UpdateDate = @UpdateDate, IsFinished = @IsFinished, History = @History where Tracking = @Tracking";


        public PackageDAO(IConfigurationService configurationService) : base(configurationService) { }

        public async Task<ICollection<PackageDTO>> LoadTrackingAsync()
        {
            using (var c = NewConnection)
            {
                return (await c.QueryAsync<Package>(LoadComingPackagesQuery)).ToList().Select(x => x.Map()).ToList();
            }
        }

        public async Task<PackageDTO> LoadAsync(string trackingNumber)
        {
            return (await Get(trackingNumber)).Map();
        }

        public async Task RegisterAsync(RegisterPackageDTO package)
        {
            using (var c = NewConnection)
            {
                try
                {
                    await c.ExecuteAsync(RegisterQuery, package.Map());
                } catch (Exception ex)
                {

                }
            }
        }

        public bool Exists(string trackingNumber)
        {
            using (var c = NewConnection)
            {
                return c.QueryFirstOrDefault<bool>(ExistsQuery, new {trackingNumber});
            }
        }

        public async Task UpdateAsync(PackageDTO package)
        {
            using (var c = NewConnection)
            {
                var entity = await Get(package.Tracking);
                package.Map(entity);
                entity.UpdateDate = DateTime.Now;
                await c.ExecuteAsync(UpdateQuery, entity);
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

        private async Task<Package> Get(string trackingNumber)
        {
            using (var c = NewConnection)
            {
                return await c.QueryFirstOrDefaultAsync<Package>(LoadQuery, new {trackingNumber});
            }
        }
    }
}