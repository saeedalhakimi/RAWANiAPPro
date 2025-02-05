using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RAWANi.WEBAPi.Domain.Entities.UserProfiles
{
    public class BasicInformation
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string CurrentCity { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Gender { get; private set; }
        private BasicInformation() { }
        public static OperationResult<BasicInformation> Create(
            string firstName, 
            string lastName, 
            string address, 
            string currentCity, 
            DateTime dateOfBirth, 
            string gender)
        {

            if (string.IsNullOrWhiteSpace(firstName) 
                || firstName.Length < 1 
                || firstName.Length > 50)
            {
                return OperationResult<BasicInformation>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "First name cannot be empty, First name must be between 1 and 50 characters."
                ));
            }

            if (string.IsNullOrWhiteSpace(lastName)
                || lastName.Length < 1
                || lastName.Length > 50)
            {
                return OperationResult<BasicInformation>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "Last name cannot be empty, Last name must be between 1 and 50 characters."
                ));
            }

            if (address.Length > 100)
            {
                return OperationResult<BasicInformation>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "Address max of 100 characters."
                ));
            }

            if (currentCity.Length > 50)
            {
                return OperationResult<BasicInformation>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "City name max of 50 characters."
                ));
            }

            if(dateOfBirth > DateTime.UtcNow.AddYears(-18) 
                || dateOfBirth < DateTime.UtcNow.AddYears(-125))
            {
                return OperationResult<BasicInformation>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "User must be at least 18 years old, and User cannot be older than 125 years.."
                ));
            }

            if (string.IsNullOrWhiteSpace(gender)
                || !(gender.Equals("Male", StringComparison.OrdinalIgnoreCase)
                     || gender.Equals("Female", StringComparison.OrdinalIgnoreCase)))
            {
                return OperationResult<BasicInformation>.Failure(new Error(
                    ErrorCode.InvalidInput,
                    "Invalid Input.",
                    "Gender cannot be empty, and gender must be one of the following: Male, Female.."
                ));
            }


            var basicInofrmation = new BasicInformation
            {
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                CurrentCity = currentCity,
                DateOfBirth = dateOfBirth,
                Gender = gender
                
            };

            return OperationResult<BasicInformation>.Success(basicInofrmation);
        }
    }
}
