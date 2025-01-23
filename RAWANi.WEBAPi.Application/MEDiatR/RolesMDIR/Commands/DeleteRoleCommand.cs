﻿using MediatR;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.RolesMDIR.Commands
{
    public class DeleteRoleCommand : IRequest<OperationResult<bool>>
    {
        public string RoleID { get; set; }
    }
}
