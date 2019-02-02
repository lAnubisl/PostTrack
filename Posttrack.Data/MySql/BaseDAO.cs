using MySql.Data.MySqlClient;
using Posttrack.Common;
using Posttrack.Data.Interfaces;

namespace Posttrack.Data.MySql
{
    public abstract class BaseDAO
    {
        private readonly string _connectionString;

        public BaseDAO(IConfigurationService configurationService, ILogger logger)
        {
            Logger = logger;
            _connectionString = configurationService.GetConnectionString();
        }

        protected ILogger Logger { get; }

        protected MySqlConnection NewConnection
        {
            get
            {
                return new MySqlConnection(_connectionString);
            }
        }
    }
}