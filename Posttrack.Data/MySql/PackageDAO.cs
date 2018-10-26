using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Posttrack.Common;
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


        public PackageDAO(IConfigurationService configurationService, ILogger logger) 
            : base(configurationService, logger.CreateScope(nameof(PackageDAO))) { }

        public async Task<ICollection<PackageDTO>> LoadTrackingAsync()
        {
            logger.Info($"Call: {nameof(LoadTrackingAsync)}()");
            using (var c = NewConnection)
            {
                try
                {
                    return (await c.QueryAsync<Package>(LoadComingPackagesQuery)).ToList().Select(x => x.Map()).ToList();
                }
                catch(Exception ex)
                {
                    logger.Log(ex);
                    return null;
                }
            }
        }

        public async Task<PackageDTO> LoadAsync(string trackingNumber)
        {
            logger.Info($"Call: {nameof(LoadAsync)}({trackingNumber})");
            return (await Get(trackingNumber)).Map();
        }

        public async Task RegisterAsync(RegisterPackageDTO package)
        {
            logger.Info($"Call: {nameof(RegisterAsync)}(package)");
            using (var c = NewConnection)
            {
                try
                {
                    await c.ExecuteAsync(RegisterQuery, package.Map());
                } catch (Exception ex)
                {
                    logger.Log(ex);
                }
            }
        }

        public bool Exists(string trackingNumber)
        {
            logger.Info($"Call: {nameof(Exists)}({trackingNumber})");
            using (var c = NewConnection)
            {
                try
                {
                    return c.QueryFirstOrDefault<bool>(ExistsQuery, new { trackingNumber });
                }
                catch (Exception ex)
                {
                    logger.Log(ex);
                    throw ex;
                }  
            }
        }

        public async Task UpdateAsync(PackageDTO package)
        {
            logger.Info($"Call: {nameof(UpdateAsync)}(package)");
            using (var c = NewConnection)
            {
                var entity = await Get(package.Tracking);
                package.Map(entity);
                entity.UpdateDate = DateTime.Now;
                await c.ExecuteAsync(UpdateQuery, entity);
            }
        }

        private async Task<Package> Get(string trackingNumber)
        {
            logger.Info($"Call: {nameof(Get)}({trackingNumber})");
            using (var c = NewConnection)
            {
                try
                {
                    return await c.QueryFirstOrDefaultAsync<Package>(LoadQuery, new { trackingNumber });
                }
                catch(Exception ex)
                {
                    logger.Log(ex);
                    return null;
                }
            }
        }
    }
}