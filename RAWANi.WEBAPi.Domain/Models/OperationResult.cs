using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.Models
{
    /// <summary>
    /// Represents the result of an operation, which can either be successful or contain errors.
    /// </summary>
    /// <typeparam name="T">The type of the payload data.</typeparam>
    public class OperationResult<T>
    {
        /// <summary>
        /// Gets the payload data of the operation. Null if the operation failed.
        /// </summary>
        public T? Data { get; private set; }

        /// <summary>
        /// Indicates whether the operation resulted in an error.
        /// </summary>
        public bool IsError { get; private set; }

        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool IsSuccess => !IsError;

        /// <summary>
        /// Gets the list of errors that occurred during the operation.
        /// </summary>
        public List<Error> Errors { get; private set; } = new List<Error>();

        /// <summary>
        /// Gets the timestamp when the operation result was created.
        /// </summary>
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Private constructor for creating a successful operation result.
        /// </summary>
        /// <param name="payload">The payload data.</param>
        private OperationResult(T payload)
        {
            Data = payload;
            IsError = false;
        }

        /// <summary>
        /// Private constructor for creating a failed operation result.
        /// </summary>
        /// <param name="errors">The list of errors.</param>
        private OperationResult(List<Error> errors)
        {
            IsError = true;
            Errors = errors;
        }

        /// <summary>
        /// Creates a successful operation result with the specified payload.
        /// </summary>
        /// <param name="payload">The payload data.</param>
        /// <returns>An instance of <see cref="OperationResult{T}"/>.</returns>
        public static OperationResult<T> Success(T payload) => new OperationResult<T>(payload);

        /// <summary>
        /// Creates a successful operation result with a tuple payload.
        /// </summary>
        /// <typeparam name="T1">The type of the first item in the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second item in the tuple.</typeparam>
        /// <param name="item1">The first item in the tuple.</param>
        /// <param name="item2">The second item in the tuple.</param>
        /// <returns>An instance of <see cref="OperationResult{T}"/>.</returns>
        public static OperationResult<(IEnumerable<T1>, T2)> Success<T1, T2>(IEnumerable<T1> item1, T2 item2)
            => new OperationResult<(IEnumerable<T1>, T2)>((item1, item2));


        /// <summary>
        /// Creates a failed operation result with the specified errors.
        /// </summary>
        /// <param name="errors">The list of errors.</param>
        /// <returns>An instance of <see cref="OperationResult{T}"/>.</returns>
        public static OperationResult<T> Failure(List<Error> errors) => new OperationResult<T>(errors);


        /// <summary>
        /// Creates a failed operation result with a single error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>An instance of <see cref="OperationResult{T}"/>.</returns>
        public static OperationResult<T> Failure(Error error) => new OperationResult<T>(new List<Error> { error });


        /// <summary>
        /// Creates a failed operation result with a single error code and message.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <returns>An instance of <see cref="OperationResult{T}"/>.</returns>
        public static OperationResult<T> Failure(ErrorCode code, string message) =>
            new OperationResult<T>(new List<Error> { new Error(code, message) });



        /// <summary>
        /// Creates a failed operation result with a single error code, message, and details.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <param name="details">The error details.</param>
        /// <returns>An instance of <see cref="OperationResult{T}"/>.</returns>
        public static OperationResult<T> Failure(ErrorCode code, string message, string? details) =>
            new OperationResult<T>(new List<Error> { new Error(code, message, details) });


        /// <summary>
        /// Checks if the operation result contains any errors.
        /// </summary>
        /// <returns>True if there are errors, otherwise false.</returns>
        public bool HasErrors() => IsError && Errors.Any();

        /// <summary>
        /// Gets a concatenated string of all error messages.
        /// </summary>
        /// <returns>A string containing all error messages.</returns>
        public string GetErrorMessage() => string.Join(", ", Errors.Select(e => e.Message));

        /// <summary>
        /// Gets the first error message, if any.
        /// </summary>
        /// <returns>The first error message or an empty string.</returns>
        public string GetFirstErrorMessage() => Errors.FirstOrDefault()?.Message ?? string.Empty;

        /// <summary>
        /// Returns a string representation of the operation result.
        /// </summary>
        /// <returns>A string describing the operation result.</returns>
        public override string ToString() =>
            IsError
                ? $"Error(s) occurred at {Timestamp}: {GetErrorMessage()}"
                : $"Success at {Timestamp}: {Data}";

    }
}
