using Dapper;
using Posttrack.Common;
using Posttrack.Data.Interfaces;
using System;

namespace Posttrack.Data.MySql
{
    public class SettingDAO : BaseDAO, ISettingDAO
    {
        public SettingDAO(IConfigurationService configurationService, ILogger logger) 
            : base(configurationService, logger.CreateScope(nameof(SettingDAO))) { }

        private const string GetQuery = "select Value from Settings where Name = @name";

        public string Get(string name)
        {
            logger.Info($"Call: {nameof(Get)}({name})");
            using (var c = NewConnection)
            {
                try
                {
                    return c.QueryFirstOrDefault<string>(GetQuery, new { name });
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