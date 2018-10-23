using MySql.Data.MySqlClient;
using Posttrack.Data.Interfaces;

namespace Posttrack.Data.MySql
{
    public abstract class BaseDAO
    {
        private readonly string _connectionString;
        protected MySqlConnection NewConnection {
            get
            {
                return new MySqlConnection(_connectionString);
            }
        }
        public BaseDAO(IConfigurationService configurationService)
        {
            _connectionString = configurationService.GetConnectionString();
        }
    }
}