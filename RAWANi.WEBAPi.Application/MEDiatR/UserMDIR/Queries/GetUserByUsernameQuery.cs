using MediatR;
using Microsoft.AspNetCore.Mvc;
using RAWANi.WEBAPi.Application.Contracts.UsersDto.Responses;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.UserMDIR.Queries
{
    public class GetUserByUsernameQuery : IRequest<OperationResult<UserResponseDto>>
    {
        [Required(ErrorMessage = "Please provide the username/email")]
        [EmailAddress]
        public string UserName { get; set; }
    }
}
