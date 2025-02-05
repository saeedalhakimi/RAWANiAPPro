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
    /// Represents a strongly-typed identifier for a Guids.
    /// </summary>
    [DebuggerDisplay("GuidID: {Value}")]
    public readonly struct GuidID : IEquatable<GuidID>
    {
        /// <summary>
        /// Gets the underlying GUID value.
        /// </summary>
        public Guid Value { get; }

        private GuidID(Guid value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="GuidID"/> instance.
        /// </summary>
        /// <param name="value">The GUID value.</param>
        /// <returns>An <see cref="OperationResult{GuidID}"/> indicating success or failure.</returns>
        public static OperationResult<GuidID> Create(Guid value)
        {
            if (value == Guid.Empty)
            {
                return OperationResult<GuidID>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "Guid cannot be empty."));
            }

            //// Optional: Validate GUID version or variant
            //if (value.Version != 4) // Example: Ensure it's a random GUID (version 4)
            //{
            //    return OperationResult<PostGuid>.Failure(new Error(
            //        ErrorCode.InvalidInput,
            //        "Invalid Input.",
            //        "Post Id must be a version 4 GUID."));
            //}

            return OperationResult<GuidID>.Success(new GuidID(value));
        }

        /// <inheritdoc />
        public override string ToString() => Value.ToString();

        /// <inheritdoc />
        public bool Equals(GuidID other) => Value == other.Value;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is GuidID other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Determines whether two <see cref="GuidID"/> instances are equal.
        /// </summary>
        public static bool operator ==(GuidID left, GuidID right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="GuidID"/> instances are not equal.
        /// </summary>
        public static bool operator !=(GuidID left, GuidID right) => !left.Equals(right);

        /// <summary>
        /// Implicitly converts a <see cref="Guid"/> to a <see cref="GuidID"/>.
        /// </summary>
        public static implicit operator GuidID(Guid value) => new(value);

        /// <summary>
        /// Implicitly converts a <see cref="GuidID"/> to a <see cref="Guid"/>.
        /// </summary>
        public static implicit operator Guid(GuidID postId) => postId.Value;

        /// <summary>
        /// Creates a new <see cref="GuidID"/> with a randomly generated GUID.
        /// </summary>
        public static GuidID CreateNew() => new(Guid.NewGuid());
        public void Deconstruct(out Guid value) => value = Value;

    }
}
