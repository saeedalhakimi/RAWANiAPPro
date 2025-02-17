using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Services;

namespace RAWANi.WEBAPi.Services
{
    public class LoggMessagingService : ILoggMessagingService
    {
        private readonly Dictionary<string, string> _errorMessages;
        private readonly Dictionary<string, string> _successMessages;
        private readonly Dictionary<string, string> _loggMessages;
        private readonly Dictionary<string, string> _warningMessages;
        private readonly Dictionary<string, string> _detailedMessage;
        public LoggMessagingService()
        {
            _errorMessages = new Dictionary<string, string>
            {
                { nameof(ErrorMessage.InvalidClaim), ErrorMessage.InvalidClaim },
                { nameof(ErrorMessage.ResourceRetrievalFailed), ErrorMessage.ResourceRetrievalFailed },

                { nameof(ErrorMessage.UnknownError), ErrorMessage.UnknownError },
                { nameof(ErrorMessage.NotFound), ErrorMessage.NotFound },
                { nameof(ErrorMessage.OperationCanceled), ErrorMessage.OperationCanceled },
                { nameof(ErrorMessage.DatabaseError), ErrorMessage.DatabaseError },
                { nameof(ErrorMessage.ErrorMappingData), ErrorMessage.ErrorMappingData },
                { nameof(ErrorMessage.CreationFailed), ErrorMessage.CreationFailed },
                { nameof(ErrorMessage.EmailConflict), ErrorMessage.EmailConflict },
                { nameof(ErrorMessage.GOVIDCONFLICT), ErrorMessage.GOVIDCONFLICT },
                { nameof(ErrorMessage.CONFLICTERROR), ErrorMessage.CONFLICTERROR },
                { nameof(ErrorMessage.ResourceAlreadyExists), ErrorMessage.ResourceAlreadyExists },

            };

            _successMessages = new Dictionary<string, string>
            {
                { nameof(SuccessMessage.QueryExecuted), SuccessMessage.QueryExecuted },
                { nameof(SuccessMessage.RequestProcessed), SuccessMessage.RequestProcessed },

                { nameof(SuccessMessage.TransactionSuccess), SuccessMessage.TransactionSuccess },
                { nameof(SuccessMessage.DataConnectionSuccess), SuccessMessage.DataConnectionSuccess },
                { nameof(SuccessMessage.RetrieveSuccess), SuccessMessage.RetrieveSuccess },
                { nameof(SuccessMessage.ResourcesMappedSuccessfully), SuccessMessage.ResourcesMappedSuccessfully },
                { nameof(SuccessMessage.MDItRProcessSuccess), SuccessMessage.MDItRProcessSuccess },
                { nameof(SuccessMessage.CreationSuccess), SuccessMessage.CreationSuccess },
                { nameof(SuccessMessage.DBCreationSuccess), SuccessMessage.DBCreationSuccess },
            };

            _loggMessages = new Dictionary<string, string>
            {
                //mdeidtr
                { nameof(LoggMessage.MDIHandlingRequest), LoggMessage.MDIHandlingRequest },
                { nameof(LoggMessage.CancellationTokenChecked), LoggMessage.CancellationTokenChecked },

                { nameof(LoggMessage.ADOHandlingRequest), LoggMessage.ADOHandlingRequest },

                //controllers
                { nameof(LoggMessage.RequestReceived), LoggMessage.RequestReceived },
                { nameof(LoggMessage.RetrievingClaims), LoggMessage.RetrievingClaims },
                { nameof(LoggMessage.ExecutingQuery), LoggMessage.ExecutingQuery },
                { nameof(LoggMessage.ExecutingCommand), LoggMessage.ExecutingCommand },

            };

            _warningMessages = new Dictionary<string, string>
            {
                { nameof(WarningMessage.InvalidClaim), WarningMessage.InvalidClaim },
            };

            _detailedMessage = new Dictionary<string, string>
            {
                { nameof(DetailedMessage.MissingOrInvalidUserProfileIdClaim), DetailedMessage.MissingOrInvalidUserProfileIdClaim },
            };
        }
        public string GetErrorMessage(string key) =>
           _errorMessages.TryGetValue(key, out var message) ? message : "Unknown error message";

        public string GetSuccessMessage(string key) =>
            _successMessages.TryGetValue(key, out var message) ? message : "Unknown success message";

        public string GetLoggMessage(string key) =>
            _loggMessages.TryGetValue(key, out var message) ? message : "Unknown success message";

        public string GetLoggMessage(string key, params object[] args)
        {
            if (_loggMessages.TryGetValue(key, out var message))
            {
                return string.Format(message, args); // Format the message with provided arguments
            }
            return "Unknown log message"; // Fallback if the key is not found
        }

        public string GetWarningMessage(string key, params object[] args)
        {
            if (_warningMessages.TryGetValue(key, out var message))
            {
                return string.Format(message, args); // Format the message with provided arguments
            }
            return "Unknown warning message"; // Fallback if the key is not found
        }

        public string GetDetailedMessage(string key, params object[] args)
        {
            if (_detailedMessage.TryGetValue(key, out var message))
            {
                return string.Format(message, args); // Format the message with provided arguments
            }
            return "Unknown warning message"; // Fallback if the key is not found
        }
    }
}
