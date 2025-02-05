using RAWANi.WEBAPi.Domain.Entities.Posts.ValueObjects;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.UnitTests.IntitiesTests.PostTest.ValueObjectsTests
{
    public class HeaderTest
    {
        [Fact]
        public void Create_WithValidValue_ReturnsSuccess()
        {
            // Arrange
            var validValue = "Valid Header";

            // Act
            var result = Header.Create(validValue);

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
            var result = Header.Create(emptyValue);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Equal("Invalid Input.", result.Errors.First().Message);
            Assert.Equal("Title cannot be empty.", result.Errors.First().Details);
        }

        [Fact]
        public void Create_WithWhitespaceValue_ReturnsFailure()
        {
            // Arrange
            var whitespaceValue = "   ";

            // Act
            var result = Header.Create(whitespaceValue);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Equal("Invalid Input.", result.Errors.First().Message);
            Assert.Equal("Title cannot be empty.", result.Errors.First().Details);
        }

        [Fact]
        public void Create_WithValueExceedingMaxLength_ReturnsFailure()
        {
            // Arrange
            var longValue = new string('a', Header.MaxLength + 1);

            // Act
            var result = Header.Create(longValue);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Equal("Invalid Input.", result.Errors.First().Message);
            Assert.Equal($"Title cannot be more than {Header.MaxLength} characters.", result.Errors.First().Details);
        }

        [Fact]
        public void Create_WithSpecialCharacters_ReturnsFailure()
        {
            // Arrange
            var specialCharValue = "Invalid@Header!";

            // Act
            var result = Header.Create(specialCharValue);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Equal("Invalid Input.", result.Errors.First().Message);
            Assert.Equal("Title cannot contain special characters.", result.Errors.First().Details);
        }

        [Fact]
        public void Equals_WithSameValue_ReturnsTrue()
        {
            // Arrange
            var value = "Test Header";
            var header1 = Header.Create(value).Data;
            var header2 = Header.Create(value).Data;

            // Act
            var areEqual = header1.Equals(header2);

            // Assert
            Assert.True(areEqual);
        }

        [Fact]
        public void Equals_WithDifferentValue_ReturnsFalse()
        {
            // Arrange
            var header1 = Header.Create("Header 1").Data;
            var header2 = Header.Create("Header 2").Data;

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
            var header = Header.Create(value).Data;

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
            var header = Header.Create(value).Data;

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
            Header header = value;

            // Assert
            Assert.Equal(value, header.Value);
        }

        [Fact]
        public void ImplicitConversion_ToString_ReturnsValue()
        {
            // Arrange
            var value = "Test Header";
            var header = Header.Create(value).Data;

            // Act
            string result = header;

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void CreateDefault_ReturnsDefaultHeader()
        {
            // Arrange
            var defaultHeader = "Untitled";

            // Act
            var header = Header.CreateDefault();

            // Assert
            Assert.Equal(defaultHeader, header.Value);
        }
    }
}
