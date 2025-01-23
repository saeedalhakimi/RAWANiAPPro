using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.RolesDto.Requests
{
    public record AssignOrRemoveRoleDto
    {
        [Required(ErrorMessage = "Please provid a user name.")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Please provid a role name.")]
        public string RoleName { get; set; }
    }
}
