using RAWANi.WEBAPi.Domain.Entities.Posts.ValueObjects;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.Entities.Posts.ValueObjects
{
    /// <summary>
    /// Represents a strongly-typed identifier for a body contents.
    /// </summary>
    [DebuggerDisplay("Bodies: {Value}")]
    public readonly struct Bodies : IEquatable<Bodies>
    {
        /// <summary>
        /// The maximum allowed length for a Title.
        /// </summary>
        public const int MaxLength = 3000;

        /// <summary>`
        /// Gets the underlying string value.
        /// </summary>
        public string Value { get; }
        private Bodies(string value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="Bodies"/> instance.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>An <see cref="OperationResult{Body}"/> indicating success or failure.</returns>
        public static OperationResult<Bodies> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return OperationResult<Bodies>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "Content cannot be empty."
                ));
            }

            if (value.Length > MaxLength)
            {
                return OperationResult<Bodies>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    $"Content cannot be more than {MaxLength} characters."
                ));
            }

            //// Optional: Validate for special characters
            //if (value.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch)))
            //{
            //    return OperationResult<Header>.Failure(new Error(
            //        ErrorCode.InvalidInput,
            //        "Invalid Input.",
            //        "Title cannot contain special characters."
            //    ));
            //}

            return OperationResult<Bodies>.Success(new Bodies(value));
        }

        /// <inheritdoc />
        public override string ToString() => Value;

        /// <inheritdoc />
        public bool Equals(Bodies other) => Value == other.Value;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Bodies other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Determines whether two <see cref="Bodies"/> instances are equal.
        /// </summary>
        public static bool operator ==(Bodies left, Bodies right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="Bodies"/> instances are not equal.
        /// </summary>
        public static bool operator !=(Bodies left, Bodies right) => !left.Equals(right);

        /// <summary>
        /// Implicitly converts a <see cref="string"/> to a <see cref="Bodies"/>.
        /// </summary>
        public static implicit operator Bodies(string value) => new Bodies(value);

        /// <summary>
        /// Implicitly converts a <see cref="Bodies"/> to a <see cref="string"/>.
        /// </summary>
        public static implicit operator string(Bodies header) => header.Value;

        /// <summary>
        /// Creates a new <see cref="Bodies"/> with a default value.
        /// </summary>
        public static Bodies CreateDefault() => new("No Content");
        public void Deconstruct(out string value) => value = Value;
    }
}
