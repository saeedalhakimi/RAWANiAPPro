using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory
{
    public class SqlDatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        public async Task<IDatabaseConnection> CreateConnectionAsync(string connectionString,
            CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(connectionString);
            return new SqlDatabaseConnection(connection);
        }
    }
}
