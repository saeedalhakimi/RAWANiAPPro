using MediatR;
using RAWANi.WEBAPi.Application.Contracts.UsersDto.Responses;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.UserMDIR.Queries
{
    public class GetUserDetailsQuery : IRequest<OperationResult<UserResponseDto>>
    {
        public string UserID { get; set; }  
    }
}
