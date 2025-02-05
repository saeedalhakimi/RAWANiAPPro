using RAWANi.WEBAPi.Domain.Entities.Posts.ValueObjects;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.UnitTests.IntitiesTests.PostTest.ValueObjectsTests
{
    public class GuidIDTests
    {
        [Fact]
        public void Create_WithValidGuid_ReturnsSuccess()
        {
            // Arrange
            var validGuid = Guid.NewGuid();

            // Act
            var result = GuidID.Create(validGuid);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(validGuid, result.Data.Value);
        }

        [Fact]
        public void Create_WithEmptyGuid_ReturnsFailure()
        {
            // Arrange
            var emptyGuid = Guid.Empty;

            // Act
            var result = GuidID.Create(emptyGuid);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("Guid cannot be empty.", result.Errors.First().Details);
        }

        [Fact]
        public void Equals_WithSameGuid_ReturnsTrue()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var result1 = GuidID.Create(guid);
            var result2 = GuidID.Create(guid);

            // Ensure both operations were successful
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);

            // Extract the PostGuid values
            var postGuid1 = result1.Data;
            var postGuid2 = result2.Data;

            // Act
            var areEqual = postGuid1.Equals(postGuid2);

            // Assert
            Assert.True(areEqual);
        }

        [Fact]
        public void Equals_WithDifferentGuid_ReturnsFalse()
        {
            // Arrange
            var result1 = GuidID.Create(Guid.NewGuid());
            var result2 = GuidID.Create(Guid.NewGuid());

            // Ensure both operations were successful
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);

            // Extract the PostGuid values
            var postGuid1 = result1.Data;
            var postGuid2 = result2.Data;

            // Act
            var areEqual = postGuid1.Equals(postGuid2);

            // Assert
            Assert.False(areEqual);
        }

        [Fact]
        public void GetHashCode_ReturnsGuidHashCode()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var result = GuidID.Create(guid);

            // Ensure the operation was successful
            Assert.True(result.IsSuccess);

            // Extract the PostGuid value
            var postGuid = result.Data;

            // Act
            var hashCode = postGuid.GetHashCode();

            // Assert
            Assert.Equal(guid.GetHashCode(), hashCode);
        }

        [Fact]
        public void ToString_ReturnsGuidString()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var result = GuidID.Create(guid);

            // Ensure the operation was successful
            Assert.True(result.IsSuccess);

            // Extract the PostGuid value
            var postGuid = result.Data;

            // Act
            var resultString = postGuid.ToString();

            // Assert
            Assert.Equal(guid.ToString(), resultString);
        }

        [Fact]
        public void ImplicitConversion_FromGuid_CreatesPostGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            GuidID postGuid = guid;

            // Assert
            Assert.Equal(guid, postGuid.Value);
        }

        [Fact]
        public void ImplicitConversion_ToGuid_ReturnsGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var postGuid = GuidID.Create(guid);

            // Act
            Guid result = postGuid.Data;

            // Assert
            Assert.Equal(guid, result);
        }

        [Fact]
        public void EqualityOperator_WithSameGuid_ReturnsTrue()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var postGuid1 = GuidID.Create(guid);
            var postGuid2 = GuidID.Create(guid);

            // Act
            var areEqual = postGuid1.Data == postGuid2.Data;

            // Assert
            Assert.True(areEqual);
        }

        [Fact]
        public void EqualityOperator_WithDifferentGuid_ReturnsFalse()
        {
            // Arrange
            var postGuid1 = GuidID.Create(Guid.NewGuid());
            var postGuid2 = GuidID.Create(Guid.NewGuid());

            // Act
            var areEqual = postGuid1.Data == postGuid2.Data;

            // Assert
            Assert.False(areEqual);
        }

        [Fact]
        public void InequalityOperator_WithSameGuid_ReturnsFalse()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var postGuid1 = GuidID.Create(guid);
            var postGuid2 = GuidID.Create(guid);

            // Act
            var areNotEqual = postGuid1.Data != postGuid2.Data;

            // Assert
            Assert.False(areNotEqual);
        }

        [Fact]
        public void InequalityOperator_WithDifferentGuid_ReturnsTrue()
        {
            // Arrange
            var postGuid1 = GuidID.Create(Guid.NewGuid());
            var postGuid2 = GuidID.Create(Guid.NewGuid());

            // Act
            var areNotEqual = postGuid1.Data != postGuid2.Data;

            // Assert
            Assert.True(areNotEqual);
        }
    }
}
