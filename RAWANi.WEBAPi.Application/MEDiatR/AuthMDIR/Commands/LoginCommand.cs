using MediatR;
using RAWANi.WEBAPi.Application.Contracts.AuthDtos.Responses;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.Commands
{
    public class LoginCommand : IRequest<OperationResult<ResponseWithTokensDto>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
