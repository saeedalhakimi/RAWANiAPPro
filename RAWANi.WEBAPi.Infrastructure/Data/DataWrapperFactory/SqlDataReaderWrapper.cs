using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory
{
    public class SqlDataReaderWrapper : IDataReader
    {
        private readonly SqlDataReader _reader;

        public SqlDataReaderWrapper(SqlDataReader reader)
        {
            _reader = reader;
        }

        public async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            return await _reader.ReadAsync(cancellationToken);
        }

        public Guid GetGuid(int ordinal)
        {
            return _reader.GetGuid(ordinal);
        }

        public string GetString(int ordinal)
        {
            return _reader.GetString(ordinal);
        }

        public DateTime GetDateTime(int ordinal)
        {
            return _reader.GetDateTime(ordinal);
        }

        public int GetOrdinal(string name)
        {
            return _reader.GetOrdinal(name);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public async Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            return await _reader.NextResultAsync(cancellationToken);
        }

        public int GetInt32(int ordinal)
        {
            return _reader.GetInt32(ordinal);
        }

        public long GetInt64(int ordinal) // New method implementation
        {
            return _reader.GetInt64(ordinal);
        }

        public bool GetBoolean(int ordinal) // New method implementation
        {
            return _reader.GetBoolean(ordinal);
        }
    }
}

