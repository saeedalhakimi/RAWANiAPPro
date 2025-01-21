using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Services;

namespace RAWANi.WEBAPi.Services
{
    public class LoggMessagingService : ILoggMessagingService
    {
        private readonly Dictionary<string, string> _errorMessages;
        private readonly Dictionary<string, string> _successMessages;
        private readonly Dictionary<string, string> _loggMessages;
        public LoggMessagingService()
        {
            _errorMessages = new Dictionary<string, string>
            {
                { nameof(ErrorMessage.UnknownError), nameof(ErrorMessage.UnknownError) },
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
                { nameof(LoggMessage.MDIHandlingRequest), LoggMessage.MDIHandlingRequest },
                { nameof(LoggMessage.ADOHandlingRequest), LoggMessage.ADOHandlingRequest },

            };
        }
        public string GetErrorMessage(string key) =>
           _errorMessages.TryGetValue(key, out var message) ? message : "Unknown error message";

        public string GetSuccessMessage(string key) =>
            _successMessages.TryGetValue(key, out var message) ? message : "Unknown success message";

        public string GetLoggMessage(string key) =>
            _loggMessages.TryGetValue(key, out var message) ? message : "Unknown success message";

    }
}
