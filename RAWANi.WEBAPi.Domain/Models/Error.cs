using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.Models
{
    public class Error
    {
        public ErrorCode Code { get; private set; }
        public string Message { get; private set; }
        public string? Details { get; private set; }  // Optional additional context for the error

        // Constructor with code, message, and optional details
        public Error(ErrorCode code, string message, string? details = null)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        // Override ToString for better logging/debugging
        public override string ToString()
        {
            return $"Error {Code}: {Message}" + (string.IsNullOrEmpty(Details) ? string.Empty :
                $" - Details: {Details}");
        }

        /// <summary>
        /// Converts the error to a formatted string suitable for logging.
        /// </summary>
        /// <param name="includeDetails">Include the details in the output if true.</param>
        /// <returns>A formatted string of the error.</returns>
        public string ToLogString(bool includeDetails = true)
        {
            return includeDetails && !string.IsNullOrEmpty(Details)
                ? $"Code: {Code}, Message: {Message}, Details: {Details}"
                : $"Code: {Code}, Message: {Message}";
        }

        /// <summary>
        /// Provides a dictionary representation of the error for logging or serialization.
        /// </summary>
        /// <returns>A dictionary containing the error details.</returns>
        public Dictionary<string, object?> ToDictionary()
        {
            return new Dictionary<string, object?>
        {
            { "Code", Code },
            { "Message", Message },
            { "Details", Details }
        };
        }
    }
}
