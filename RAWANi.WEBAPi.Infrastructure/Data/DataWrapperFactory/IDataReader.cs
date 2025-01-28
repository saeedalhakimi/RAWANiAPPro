using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory
{
    public interface IDataReader : IDisposable
    {
        Task<bool> ReadAsync(CancellationToken cancellationToken);
        Task<bool> NextResultAsync(CancellationToken cancellationToken);
        Guid GetGuid(int ordinal);
        string GetString(int ordinal);
        DateTime GetDateTime(int ordinal);
        int GetOrdinal(string name);
        int GetInt32(int ordinal);
        long GetInt64(int ordinal); // New method to handle long values
        bool GetBoolean(int ordinal); // New method to handle boolean values
    }
}
