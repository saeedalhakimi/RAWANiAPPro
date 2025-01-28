using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.Entities.UserProfiles
{
    public class BasicInformation
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Address { get; private set; }
        public string CurrentCity { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Gender { get; private set; }
        private BasicInformation() { }
        public static OperationResult<BasicInformation> Create(
            string firstName, string lastName, 
            string email, string phone, string address, string currentCity, DateTime dateOfBirth, string gender)
        {
            var basicInofrmation = new BasicInformation
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Address = address,
                CurrentCity = currentCity,
                DateOfBirth = dateOfBirth,
                Gender = gender
                
            };

            return OperationResult<BasicInformation>.Success(basicInofrmation);
        }
    }
}
