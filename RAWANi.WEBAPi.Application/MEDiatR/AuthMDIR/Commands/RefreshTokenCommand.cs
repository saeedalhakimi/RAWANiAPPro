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
    public class RefreshTokenCommand : IRequest<OperationResult<ResponseWithTokensDto>>
    {
        public string RefreshToken { get; set; }
    }
}
