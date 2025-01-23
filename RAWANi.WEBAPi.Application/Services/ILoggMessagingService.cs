using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Services
{
    public interface ILoggMessagingService
    {
        string GetErrorMessage(string key);
        string GetSuccessMessage(string key);
        string GetLoggMessage(string key);
        string GetLoggMessage(string key, params object[] args); // New overload to support formatting
    }
}
