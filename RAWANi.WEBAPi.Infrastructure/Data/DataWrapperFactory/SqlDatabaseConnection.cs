using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory
{
    public class SqlDatabaseConnection : IDatabaseConnection
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;

        public SqlDatabaseConnection(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                await _connection.OpenAsync(cancellationToken);
            }
        }

        public IDbCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            if (_transaction != null)
            {
                command.Transaction = _transaction;
            }
            return new SqlDbCommand(command);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            // Explicitly cast the result to SqlTransaction
            _transaction = (SqlTransaction)await _connection.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction to commit.");
            }

            await _transaction.CommitAsync(cancellationToken);
            _transaction = null; // Clear the transaction after committing
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction to rollback.");
            }

            await _transaction.RollbackAsync(cancellationToken);
            _transaction = null; // Clear the transaction after rolling back
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            await _connection.DisposeAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection.Dispose();
        }
    }
}
