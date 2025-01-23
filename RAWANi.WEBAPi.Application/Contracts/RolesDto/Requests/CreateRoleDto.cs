using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.RolesDto.Requests
{
    public record CreateRoleDto
    {
        [Required(ErrorMessage = "Role name required.")]
        public string RoleName { get; set; }
    }
}
