using MediatR;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Commands
{
    public class RemoveRoleFromUserCommand : IRequest<OperationResult<bool>> 
    {
        public string UserID { get; set; }
        public string RoleName { get; set; }
    }
}
