using System;
using Dapper;
using Posttrack.Common;
using Posttrack.Data.Interfaces;

namespace Posttrack.Data.MySql
{
    public class SettingDAO : BaseDAO, ISettingDAO
    {
        private const string GetQuery = "select Value from Settings where Name = @name";

        public SettingDAO(IConfigurationService configurationService, ILogger logger)
            : base(configurationService, logger.CreateScope(nameof(SettingDAO)))
        {
        }

        public string Load(string name)
        {
            Logger.Info($"Call: {nameof(Load)}({name})");
            using (var c = NewConnection)
            {
                try
                {
                    return c.QueryFirstOrDefault<string>(GetQuery, new { name });
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return null;
                }
            }
        }
    }
}