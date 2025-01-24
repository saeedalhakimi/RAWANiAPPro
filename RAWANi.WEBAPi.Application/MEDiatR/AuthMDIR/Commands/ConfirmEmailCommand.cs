using MediatR;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.Commands
{
    public class ConfirmEmailCommand : IRequest<OperationResult<bool>>
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
