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
    /// Represents a strongly-typed identifier for a UserProfile entity.
    /// </summary>
    [DebuggerDisplay("UserProfileGuid: {Value}")]
    public readonly struct UserProfileGuid : IEquatable<UserProfileGuid>
    {
        /// <summary>
        /// Gets the underlying GUID value.
        /// </summary>
        public Guid Value { get; }


        private UserProfileGuid(Guid value) => Value = value;

        /// <summary>
        /// Creates a new <see cref="UserProfileGuid"/> instance.
        /// </summary>
        /// <param name="value">The GUID value.</param>
        /// <returns>An <see cref="OperationResult{UserProfileGuid}"/> indicating success or failure.</returns>
        public static OperationResult<UserProfileGuid> Create(Guid value)
        {
            if (value == Guid.Empty)
            {
                return OperationResult<UserProfileGuid>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "User profile Id cannot be empty."));
            }

            return OperationResult<UserProfileGuid>.Success(new UserProfileGuid(value));
        }

        /// <inheritdoc />
        public override string ToString() => Value.ToString();

        /// <inheritdoc />
        public bool Equals(UserProfileGuid other) => Value == other.Value;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is UserProfileGuid other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Determines whether two <see cref="UserProfileGuid"/> instances are equal.
        /// </summary>
        public static bool operator ==(UserProfileGuid left, UserProfileGuid right) => left.Equals(right);

        /// <summary>
        /// Determines whether two <see cref="UserProfileGuid"/> instances are not equal.
        /// </summary>
        public static bool operator !=(UserProfileGuid left, UserProfileGuid right) => !left.Equals(right);

        /// <summary>
        /// Implicitly converts a <see cref="Guid"/> to a <see cref="UserProfileGuid"/>.
        /// </summary>
        public static implicit operator UserProfileGuid(Guid value) => new UserProfileGuid(value);

        /// <summary>
        /// Implicitly converts a <see cref="UserProfileGuid"/> to a <see cref="Guid"/>.
        /// </summary>
        public static implicit operator Guid(UserProfileGuid userProfileId) => userProfileId.Value;

        /// <summary>
        /// Creates a new <see cref="UserProfileGuid"/> with a randomly generated GUID.
        /// </summary>
        public static UserProfileGuid CreateNew() => new(Guid.NewGuid());
        public void Deconstruct(out Guid value) => value = Value;
    }
}
