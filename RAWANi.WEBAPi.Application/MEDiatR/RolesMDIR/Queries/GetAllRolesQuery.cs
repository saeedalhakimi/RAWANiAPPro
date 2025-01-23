using MediatR;
using Microsoft.AspNetCore.Identity;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Queries
{
    public class GetAllRolesQuery : IRequest<OperationResult<IEnumerable<IdentityRole>>>
    {
    }
}
