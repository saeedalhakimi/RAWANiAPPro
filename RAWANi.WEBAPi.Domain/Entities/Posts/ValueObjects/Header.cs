using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.Entities.Posts.ValueObjects
{
    /// <summary>
    /// Represents a strongly-typed identifier for a Title entities.
    /// </summary>
    [DebuggerDisplay("Header: {Value}")]
    public readonly struct Header : IEquatable<Header>
    {
        /// <summary>
        /// The maximum allowed length for a Title.
        /// </summary>
        public const int MaxLength = 70;

        /// <summary>
        /// Gets the underlying string value.
        /// </summary>
        public string Value { get; }
        private Header(string value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="Header"/> instance.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>An <see cref="OperationResult{Header}"/> indicating success or failure.</returns>
        public static OperationResult<Header> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return OperationResult<Header>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "Title cannot be empty."
                ));
            }

            if (value.Length > MaxLength)
            {
                return OperationResult<Header>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    $"Title cannot be more than {MaxLength} characters."
                ));
            }

            // Optional: Validate for special characters
            if (value.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch)))
            {
                return OperationResult<Header>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "Title cannot contain special characters."
                ));
            }

            return OperationResult<Header>.Success(new Header(value));
        }

        /// <inheritdoc />
        public override string ToString() => Value;

        /// <inheritdoc />
        public bool Equals(Header other) => Value == other.Value;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Header other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();


        /// <summary>
        /// Determines whether two <see cref="Header"/> instances are equal.
        /// </summary>
        public static bool operator ==(Header left, Header right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Header"/> instances are not equal.
        /// </summary>
        public static bool operator !=(Header left, Header right) => !left.Equals(right);

        /// <summary>
        /// Implicitly converts a <see cref="string"/> to a <see cref="Header"/>.
        /// </summary>
        public static implicit operator Header(string value) => new Header(value);

        /// <summary>
        /// Implicitly converts a <see cref="Header"/> to a <see cref="string"/>.
        /// </summary>
        public static implicit operator string(Header header) => header.Value;

        /// <summary>
        /// Creates a new <see cref="Header"/> with a default value.
        /// </summary>
        public static Header CreateDefault() => new("Untitled");
        public void Deconstruct(out string value) => value = Value;
    }
}

