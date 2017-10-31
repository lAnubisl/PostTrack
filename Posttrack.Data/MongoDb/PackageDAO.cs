using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Posttrack.Data.Interfaces;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.Data.MongoDb
{
    public class PackageDAO : IPackageDAO
    {
        private readonly IMongoCollection<Package> packages;

        public PackageDAO(string connectionString)
        {
            var url = MongoUrl.Create(connectionString);
            var client = new MongoClient(url);
            packages = client.GetDatabase(url.DatabaseName).GetCollection<Package>("packages");
        }

        ICollection<PackageDTO> IPackageDAO.LoadComingPackets()
        {
            var builder = Builders<Package>.Filter;
            var filter = builder.Or(builder.Eq(p => p.IsFinished, false), builder.Exists(p => p.IsFinished));
            return packages.Find(filter).ToList().Select(x => x.Map()).ToList();
        }

        void IPackageDAO.Register(RegisterPackageDTO package)
        {
            var entity = package.Map();
            entity.CreateDate = DateTime.Now;
            Save(entity);
        }

        bool IPackageDAO.Exists(string trackingNumber)
        {
            var builder = Builders<Package>.Filter;
            var filter = builder.Eq(p => p.Tracking, trackingNumber);
            return packages.Count(filter) > 0;
        }

        PackageDTO IPackageDAO.Load(string trackingNumber)
        {
            var builder = Builders<Package>.Filter;
            var filter = builder.Eq(p => p.Tracking, trackingNumber);
            var package = packages.Find(filter).FirstOrDefault();
            if (package == null) return null;
            return package.Map();
        }

        void IPackageDAO.Update(PackageDTO package)
        {
            if (string.IsNullOrWhiteSpace(package?.Tracking)) return;
            var builder = Builders<Package>.Filter;
            var filter = builder.Eq(p => p.Tracking, package.Tracking);
            var entity = packages.Find(filter).FirstOrDefault();
            if (entity == null) return;
            package.Map(entity);
            Save(entity);
        }

        private void Save(Package entity)
        {
            entity.UpdateDate = DateTime.Now;
            var builder = Builders<Package>.Filter;
            var filter = builder.Eq(p => p.Tracking, entity.Tracking);
            packages.ReplaceOne(filter, entity, new UpdateOptions { IsUpsert = true });
        }

        public void Insert(PackageDTO package)
        {
            packages.InsertOne(new Package
            {
                History = package.History?.Select(h => new PackageHistoryItem
                {
                    Action = h.Action,
                    Date = h.Date,
                    Place = h.Place
                }).ToList(),
                CreateDate = DateTime.Now,
                Tracking = package.Tracking,
                Description = package.Description,
                Email = package.Email,
                IsFinished = package.IsFinished,
                UpdateDate = package.UpdateDate
            });
        }
    }
}