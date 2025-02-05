using RAWANi.WEBAPi.Domain.Entities.UserProfiles;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.UnitTests.IntitiesTests.UserProfileTests
{
    public class BasicInformationTests
    {
        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            string address = "123 Main St";
            string currentCity = "New York";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string gender = "Male";

            // Act
            var result = BasicInformation.Create(
                firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsError);
            Assert.Equal(firstName, result.Data.FirstName);
            Assert.Equal(lastName, result.Data.LastName);
            Assert.Equal(address, result.Data.Address);
            Assert.Equal(currentCity, result.Data.CurrentCity);
            Assert.Equal(dateOfBirth, result.Data.DateOfBirth);
            Assert.Equal(gender, result.Data.Gender);
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenFirstNameIsEmpty()
        {
            // Arrange
            string firstName = ""; // Invalid: Empty
            string lastName = "Doe";
            string address = "123 Main St";
            string currentCity = "New York";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string gender = "Male";

            // Act
            var result = BasicInformation.Create(firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("First name cannot be empty, First name must be between 1 and 50 characters."
                , result.Errors.First().Details);
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenFirstNameExceeds50Characters()
        {
            // Arrange
            string firstName = new string('a', 51); //Not valid
            string lastName = "Doe";
            string address = "123 Main St";
            string currentCity = "New York";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string gender = "Male";

            // Act
            var result = BasicInformation.Create(firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("First name cannot be empty, First name must be between 1 and 50 characters."
                , result.Errors.First().Details);
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenLastNameIsEmpty()
        {
            // Arrange
            string firstName = "John"; 
            string lastName = ""; // Invalid: Empty
            string address = "123 Main St";
            string currentCity = "New York";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string gender = "Male";

            // Act
            var result = BasicInformation.Create(firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("Last name cannot be empty, Last name must be between 1 and 50 characters."
                , result.Errors.First().Details);
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenLastNameExceeds50Characters()
        {
            // Arrange
            string firstName = "John";
            string lastName = new string('a', 51); //Not valid
            string address = "123 Main St";
            string currentCity = "New York";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string gender = "Male";

            // Act
            var result = BasicInformation.Create(firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("Last name cannot be empty, Last name must be between 1 and 50 characters."
                , result.Errors.First().Details);
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenAddressExceeds100Characters()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            string address = new string('a', 101); //Not valid
            string currentCity = "New York";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string gender = "Male";

            // Act
            var result = BasicInformation.Create(firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("Address max of 100 characters.", result.Errors.First().Details);
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenCityNameExceeds50Characters()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            string address = "Manama street";
            string currentCity = new string('a', 51); //Not valid
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string gender = "Male";

            // Act
            var result = BasicInformation.Create(firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("City name max of 50 characters.", result.Errors.First().Details);
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenAgeIsUnder18()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            string address = "Manama street";
            string currentCity = "Manama";
            DateTime dateOfBirth = DateTime.UtcNow.AddYears(-17); // Invalid: Under 18
            string gender = "Male";

            // Act
            var result = BasicInformation.Create(firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("User must be at least 18 years old, and User cannot be older than 125 years.."
                , result.Errors.First().Details);
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenAgeIsOver125()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            string address = "Manama street";
            string currentCity = "Manama";
            DateTime dateOfBirth = DateTime.UtcNow.AddYears(-126); // Invalid: Over 125
            string gender = "Male";

            // Act
            var result = BasicInformation.Create(firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("User must be at least 18 years old, and User cannot be older than 125 years..",
                result.Errors.First().Details);
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenGenderIsEmpty()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            string address = "123 Main St";
            string currentCity = "New York";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string gender = ""; // Invalid: Not "Male" or "Female"

            // Act
            var result = BasicInformation.Create(firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("Gender cannot be empty, and gender must be one of the following: Male, Female..",
                result.Errors.First().Details);
        }

        [Fact]
        public void Create_ShouldReturnFailure_WhenGenderNotMaleOrFemale()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            string address = "123 Main St";
            string currentCity = "New York";
            DateTime dateOfBirth = new DateTime(1990, 1, 1);
            string gender = "Unknown"; // Invalid: Not "Male" or "Female"

            // Act
            var result = BasicInformation.Create(firstName, lastName, address, currentCity, dateOfBirth, gender);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("Gender cannot be empty, and gender must be one of the following: Male, Female..",
                result.Errors.First().Details);
        }
    }
    
}
