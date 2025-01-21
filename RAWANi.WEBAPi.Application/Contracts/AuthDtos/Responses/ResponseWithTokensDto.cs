using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.AuthDtos.Responses
{
    public record ResponseWithTokensDto
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
    }
}
