using Dapper;
using Posttrack.Data.Interfaces;

namespace Posttrack.Data.MySql
{
    public class SettingDAO : BaseDAO, ISettingDAO
    {
        public SettingDAO(IConfigurationService configurationService) : base(configurationService) { }

        private const string GetQuery = "select Value from Settings where Name = @name";

        public string Get(string name)
        {
            using (var c = NewConnection)
            {
                return c.QueryFirstOrDefault<string>(GetQuery, new { name });
            }
        }
    }
}