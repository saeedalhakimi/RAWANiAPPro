using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Abstractions
{
    public interface IErrorHandler
    {
        OperationResult<T> ResourceAlreadyExists<T>(string key);
        OperationResult<T> ResourceCreationFailed<T>();
        OperationResult<T> HandleException<T>(Exception ex);
        OperationResult<T> HandleCancelationToken<T>(OperationCanceledException ex);
    }
}
