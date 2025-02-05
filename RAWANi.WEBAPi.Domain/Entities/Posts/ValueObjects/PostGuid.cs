using RAWANi.WEBAPi.Domain.Models;
using System.Diagnostics;

/// <summary>
/// Represents a strongly-typed identifier for a Post entity.
/// </summary>
[DebuggerDisplay("PostGuid: {Value}")]
public readonly struct PostGuid : IEquatable<PostGuid>
{
    /// <summary>
    /// Gets the underlying GUID value.
    /// </summary>
    public Guid Value { get; }

    private PostGuid(Guid value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="PostGuid"/> instance.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>An <see cref="OperationResult{PostGuid}"/> indicating success or failure.</returns>
    public static OperationResult<PostGuid> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            return OperationResult<PostGuid>.Failure(new Error(
                ErrorCode.InvalidInput,
                "Invalid Input.",
                "Post Id cannot be empty."));
        }

        //// Optional: Validate GUID version or variant
        //if (value.Version != 4) // Example: Ensure it's a random GUID (version 4)
        //{
        //    return OperationResult<PostGuid>.Failure(new Error(
        //        ErrorCode.InvalidInput,
        //        "Invalid Input.",
        //        "Post Id must be a version 4 GUID."));
        //}

        return OperationResult<PostGuid>.Success(new PostGuid(value));
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <inheritdoc />
    public bool Equals(PostGuid other) => Value == other.Value;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is PostGuid other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="PostGuid"/> instances are equal.
    /// </summary>
    public static bool operator ==(PostGuid left, PostGuid right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PostGuid"/> instances are not equal.
    /// </summary>
    public static bool operator !=(PostGuid left, PostGuid right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a <see cref="Guid"/> to a <see cref="PostGuid"/>.
    /// </summary>
    public static implicit operator PostGuid(Guid value) => new(value);

    /// <summary>
    /// Implicitly converts a <see cref="PostGuid"/> to a <see cref="Guid"/>.
    /// </summary>
    public static implicit operator Guid(PostGuid postId) => postId.Value;

    /// <summary>
    /// Creates a new <see cref="PostGuid"/> with a randomly generated GUID.
    /// </summary>
    public static PostGuid CreateNew() => new(Guid.NewGuid());
    public void Deconstruct(out Guid value) => value = Value;

}