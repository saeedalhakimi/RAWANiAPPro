using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.Models
{
    /// <summary>
    /// Represents an error that occurred during an operation.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Gets the error code.
        /// </summary>
        public ErrorCode Code { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets additional details about the error.
        /// </summary>
        public string? Details { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <param name="details">Additional details about the error.</param>
        public Error(ErrorCode code, string message, string? details = null)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        /// <summary>
        /// Returns a string representation of the error.
        /// </summary>
        /// <returns>A string describing the error.</returns>
        public override string ToString() =>
            string.IsNullOrEmpty(Details)
                ? $"Error {Code}: {Message}"
                : $"Error {Code}: {Message} - Details: {Details}";

        /// <summary>
        /// Converts the error to a formatted string suitable for logging.
        /// </summary>
        /// <param name="includeDetails">Whether to include details in the output.</param>
        /// <returns>A formatted string of the error.</returns>
        public string ToLogString(bool includeDetails = true) =>
            includeDetails && !string.IsNullOrEmpty(Details)
                ? $"Code: {Code}, Message: {Message}, Details: {Details}"
                : $"Code: {Code}, Message: {Message}";

        /// <summary>
        /// Converts the error to a dictionary for logging or serialization.
        /// </summary>
        /// <returns>A dictionary containing the error details.</returns>
        public Dictionary<string, object?> ToDictionary() =>
            new Dictionary<string, object?>
            {
                { "Code", Code },
                { "Message", Message },
                { "Details", Details }
            };
    }
}
