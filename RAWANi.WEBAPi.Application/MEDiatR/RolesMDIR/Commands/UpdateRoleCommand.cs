using MediatR;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Commands
{
    public class UpdateRoleCommand : IRequest<OperationResult<bool>>
    {
        public string RoleID { get; set; }
        public string NewRoleName { get; set; }
    }
}
