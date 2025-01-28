using MediatR;
using Microsoft.AspNetCore.Http;
using RAWANi.WEBAPi.Application.Contracts.AuthDtos.Responses;
using RAWANi.WEBAPi.Application.Filters;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.Commands
{
    public class RegisterIdentityCommand : IRequest<OperationResult<ResponseWithTokensDto>>
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters.")]
        public string LastName { get; set; }

        [StringLength(20, ErrorMessage = "Phone max of 20 characters.")]
        public string PhoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "Address max of 100 characters.")]
        public string Address { get; set; }

        [StringLength(50, ErrorMessage = "City name max of 50 characters.")]
        public string CurrentCity { get; set; }

        [Required]
        [AgeRange(18, 125)] // Validate age is between 18 and 125 years
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [GenderValidation(ErrorMessage = "Gender must be one of the following: Male, Female.")]
        public string Gender { get; set; }
        public IFormFile ProfilePicture { get; set; } // Add this property
    }
}
