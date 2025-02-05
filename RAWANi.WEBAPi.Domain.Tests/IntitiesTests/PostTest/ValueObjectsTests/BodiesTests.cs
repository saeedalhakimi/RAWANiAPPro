using RAWANi.WEBAPi.Domain.Entities.Posts.ValueObjects;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.UnitTests.IntitiesTests.PostTest.ValueObjectsTests
{
    public class BodiesTests
    {
        [Fact]
        public void Create_WithValidValue_ReturnsSuccess()
        {
            // Arrange
            var validValue = "Valid Header";

            // Act
            var result = Bodies.Create(validValue);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(validValue, result.Data.Value);
        }

        [Fact]
        public void Create_WithEmptyValue_ReturnsFailure()
        {
            // Arrange
            var emptyValue = string.Empty;

            // Act
            var result = Bodies.Create(emptyValue);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Equal("Invalid Input.", result.Errors.First().Message);
            Assert.Equal("Content cannot be empty.", result.Errors.First().Details);
        }

        [Fact]
        public void Create_WithWhitespaceValue_ReturnsFailure()
        {
            // Arrange
            var whitespaceValue = "   ";

            // Act
            var result = Bodies.Create(whitespaceValue);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Equal("Invalid Input.", result.Errors.First().Message);
            Assert.Equal("Content cannot be empty.", result.Errors.First().Details);
        }

        [Fact]
        public void Create_WithValueExceedingMaxLength_ReturnsFailure()
        {
            // Arrange
            var longValue = new string('a', Bodies.MaxLength + 1);

            // Act
            var result = Bodies.Create(longValue);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Equal("Invalid Input.", result.Errors.First().Message);
            Assert.Equal($"Content cannot be more than {Bodies.MaxLength} characters.", result.Errors.First().Details);
        }

        [Fact]
        public void Equals_WithSameValue_ReturnsTrue()
        {
            // Arrange
            var value = "Test Header";
            var header1 = Bodies.Create(value).Data;
            var header2 = Bodies.Create(value).Data;

            // Act
            var areEqual = header1.Equals(header2);

            // Assert
            Assert.True(areEqual);
        }

        [Fact]
        public void Equals_WithDifferentValue_ReturnsFalse()
        {
            // Arrange
            var header1 = Bodies.Create("Header 1").Data;
            var header2 = Bodies.Create("Header 2").Data;

            // Act
            var areEqual = header1.Equals(header2);

            // Assert
            Assert.False(areEqual);
        }

        [Fact]
        public void GetHashCode_ReturnsValueHashCode()
        {
            // Arrange
            var value = "Test Header";
            var header = Bodies.Create(value).Data;

            // Act
            var hashCode = header.GetHashCode();

            // Assert
            Assert.Equal(value.GetHashCode(), hashCode);
        }

        [Fact]
        public void ToString_ReturnsValue()
        {
            // Arrange
            var value = "Test Header";
            var header = Bodies.Create(value).Data;

            // Act
            var result = header.ToString();

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void ImplicitConversion_FromString_CreatesHeader()
        {
            // Arrange
            var value = "Test Header";

            // Act
            Bodies header = value;

            // Assert
            Assert.Equal(value, header.Value);
        }

        [Fact]
        public void ImplicitConversion_ToString_ReturnsValue()
        {
            // Arrange
            var value = "Test Header";
            var header = Bodies.Create(value).Data;

            // Act
            string result = header;

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void CreateDefault_ReturnsDefaultHeader()
        {
            // Arrange
            var defaultHeader = "No Content";

            // Act
            var header = Bodies.CreateDefault();

            // Assert
            Assert.Equal(defaultHeader, header.Value);
        }
    }
}
