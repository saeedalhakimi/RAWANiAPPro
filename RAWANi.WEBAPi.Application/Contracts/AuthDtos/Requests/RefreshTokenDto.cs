using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.AuthDtos.Requests
{
    public record RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh token required.")]
        public string RefreshToken { get; set; }
    }
}
