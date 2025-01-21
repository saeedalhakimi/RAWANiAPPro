using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.Models
{
    public class OperationResult<T>
    {
        public T? Payload { get; private set; }
        public bool IsError { get; private set; }
        public bool IsSuccess => !IsError; // New IsSuccess property
        public List<Error> Errors { get; private set; } = new List<Error>();
        public DateTime? Timestamp { get; private set; }

        // Constructor for success
        private OperationResult(T playload)
        {
            Payload = playload;
            IsError = false;
            Timestamp = DateTime.UtcNow;
        }

        // Constructor for failure
        private OperationResult(List<Error> error)
        {
            IsError = true;
            Errors = error;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Factory method for success result.
        /// </summary>
        /// <param name="payload">The payload of the operation.</param>
        /// <returns>An instance of Result with the payload.</returns>
        public static OperationResult<T> Success(T payload) => new OperationResult<T>(payload);

        /// <summary>
        /// Factory method for success result with a tuple payload.
        /// </summary>
        /// <param name="item1">The first item of the tuple.</param>
        /// <param name="item2">The second item of the tuple.</param>
        /// <returns>An instance of Result with the tuple payload.</returns>
        public static OperationResult<(IEnumerable<T1>, T2)> Success<T1, T2>(IEnumerable<T1> item1, T2 item2)
            => new OperationResult<(IEnumerable<T1>, T2)>((item1, item2));

        /// <summary>
        /// Factory method for failure result with a list of errors.
        /// </summary>
        /// <param name="errors">The list of errors.</param>
        /// <returns>An instance of Result with the errors.</returns>
        public static OperationResult<T> Failure(List<Error> errors) => new OperationResult<T>(errors);

        /// <summary>
        /// Factory method for failure result with a single error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>An instance of Result with the error.</returns>
        public static OperationResult<T> Failure(Error error)
        {
            return new OperationResult<T>(new List<Error> { error });
        }

        /// <summary>
        /// Factory method for failure result with a single error code and message.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <returns>An instance of Result with the error.</returns>
        public static OperationResult<T> Failure(ErrorCode code, string message)
        {
            return new OperationResult<T>(new List<Error> { new Error(code, message) });
        }

        /// <summary>
        /// Factory method for failure result with a single error code and message.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <param name="details">The error details.</param>
        /// <returns>An instance of Result with the error.</returns>
        public static OperationResult<T> Failure(ErrorCode code, string message, string? details)
        {
            return new OperationResult<T>(new List<Error> { new Error(code, message, details) });
        }

        /// <summary>
        /// Check if the result has errors.
        /// </summary>
        /// <returns>True if there are errors, otherwise false.</returns>
        public bool HasErrors() => IsError && Errors.Count > 0;

        /// <summary>
        /// Get a friendly error message.
        /// </summary>
        /// <returns>A concatenated string of all error messages.</returns>
        public string GetErrorMessage() => Errors.Count > 0 ? string.Join(", ", Errors.Select(e => e.Message)) : string.Empty;

        /// <summary>
        /// Get the first error message, if any.
        /// </summary>
        /// <returns>The first error message or an empty string.</returns>
        public string GetFirstErrorMessage() => Errors.FirstOrDefault()?.Message ?? string.Empty;

        /// <summary>
        /// Format result for logging or debugging.
        /// </summary>
        /// <returns>A string representation of the domain result.</returns>
        public override string ToString()
        {
            return IsError
                ? $"Error(s) occurred at {Timestamp}: {string.Join(", ", Errors.Select(e => e.Message))}"
                : $"Success at {Timestamp}: {Payload}";
        }

    }
}
