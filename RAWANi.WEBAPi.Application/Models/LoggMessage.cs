using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Models
{
    public class LoggMessage
    {
        //controllers
        public const string RequestReceived = ": Request received... Starting process for ";
        public const string RetrievingClaims = ": Retrieving claims for the current user...";
        public const string ExecutingQuery = ": Executing query to ";
        public const string ExecutingCommand = ": Executing command...";

        //MDIR
        public const string CancellationTokenChecked = "Cancellation token verified. Proceeding with the process...";


        public const string MDIHandlingRequest = ": MediatR is processing the request...";
        public const string ADOHandlingRequest = ": ADO.NET is handling the request...";

        //public const string TransactionInit = "Beginning transaction process...";
        //public const string DatabaseConnectionEstablished = "Successfully connected to the database.";

    }
}
