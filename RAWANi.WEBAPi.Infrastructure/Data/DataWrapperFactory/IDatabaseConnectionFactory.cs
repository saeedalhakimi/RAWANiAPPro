using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory
{
    public interface IDatabaseConnectionFactory
    {
        Task<IDatabaseConnection> CreateConnectionAsync(string connectionString, CancellationToken cancellationToken);
    }
}
