using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Posttrack.Data.Entities;
using Posttrack.Data.Interfaces;
using Posttrack.Data.Interfaces.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Posttrack.Data
{
    public class PackageDAO : IPackageDAO
    {
        private readonly MongoDatabase database;
        private readonly MongoCollection<Package> packages;

        public PackageDAO(string connectionString)
        {
            var url = MongoUrl.Create(connectionString);
            var client = new MongoClient(url);
            var server = client.GetServer();
            database = server.GetDatabase(url.DatabaseName);
            packages = database.GetCollection<Package>("packages");
        }

        ICollection<PackageDTO> IPackageDAO.LoadComingPackets()
        {
            var result = new Collection<PackageDTO>();
            var query = Query.Or(Query<Package>.NotExists(e => e.IsFinished), Query<Package>.EQ(e => e.IsFinished, false));
            var items = packages.Find(query).ToList();
            foreach (var item in items)
            {
                result.Add(item.Map());
            }
            return result;
        }

        void IPackageDAO.Register(RegisterPackageDTO package)
        {
            var entity = package.Map();
            entity.CreateDate = DateTime.Now;
            Save(entity); 
        }

        bool IPackageDAO.Exists(string trackingNumber)
        {
            return packages.Count(Query<Package>.EQ(e => e.Tracking, trackingNumber)) > 0;
        }

        PackageDTO IPackageDAO.Load(string trackingNumber)
        {
            var package = packages.Find(Query<Package>.EQ(e => e.Tracking, trackingNumber)).First();
            return package.Map();
        }

        void IPackageDAO.Update(PackageDTO package)
        {
            if (package == null || string.IsNullOrWhiteSpace(package.Tracking)) return;
            var entity = packages.FindOne(Query<Package>.EQ(e => e.Tracking, package.Tracking));
            if (entity == null) return;
            package.Map(entity);
            Save(entity);
        }

        private void Save(Package entity)
        {
            entity.UpdateDate = DateTime.Now;
            packages.Save(entity);
        }
    }
}