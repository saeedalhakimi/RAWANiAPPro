using MediatR;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Commands
{
    public class CreateRoleCommand : IRequest<OperationResult<bool>>
    {
        public string RoleName { get; set; }
    }
}
