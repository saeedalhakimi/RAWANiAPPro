using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory
{
    public class SqlDbCommand : IDbCommand
    {
        private readonly SqlCommand _command;

        public SqlDbCommand(SqlCommand command)
        {
            _command = command;
        }

        public string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        public CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }

        public void AddParameter(string name, object value)
        {
            _command.Parameters.AddWithValue(name, value);
        }

        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            return await _command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<IDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
        {
            var reader = await _command.ExecuteReaderAsync(cancellationToken);
            return new SqlDataReaderWrapper(reader);
        }

        public async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return await _command.ExecuteScalarAsync(cancellationToken);
        }

        public void Dispose()
        {
            _command.Dispose();
        }
    }
}
