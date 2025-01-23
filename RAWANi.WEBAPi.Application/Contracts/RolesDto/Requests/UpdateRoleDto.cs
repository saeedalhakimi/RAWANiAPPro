using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.RolesDto.Requests
{
    public record UpdateRoleDto
    {
        [Required(ErrorMessage = "New Role name required.")]
        public string NewRoleName { get; set; }
    }
}
