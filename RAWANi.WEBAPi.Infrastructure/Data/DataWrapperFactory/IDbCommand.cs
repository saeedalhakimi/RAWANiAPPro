using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory
{
    public interface IDbCommand : IDisposable
    {
        string CommandText { get; set; }
        CommandType CommandType { get; set; }
        void AddParameter(string name, object value);
        Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken);
        Task<IDataReader> ExecuteReaderAsync(CancellationToken cancellationToken);
        Task<object> ExecuteScalarAsync(CancellationToken cancellationToken);
    }
}
