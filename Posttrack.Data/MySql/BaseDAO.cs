using MySql.Data.MySqlClient;
using Posttrack.Common;
using Posttrack.Data.Interfaces;

namespace Posttrack.Data.MySql
{
    public abstract class BaseDAO
    {
        private readonly string _connectionString;
        protected readonly ILogger logger;
        protected MySqlConnection NewConnection
        {
            get
            {
                return new MySqlConnection(_connectionString);
            }
        }

        public BaseDAO(IConfigurationService configurationService, ILogger logger)
        {
            this.logger = logger;
            _connectionString = configurationService.GetConnectionString();
        }
    }
}