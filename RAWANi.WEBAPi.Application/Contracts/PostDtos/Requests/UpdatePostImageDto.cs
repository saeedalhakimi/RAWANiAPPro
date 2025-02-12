using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.PostDtos.Requests
{
    public record UpdatePostImageDto
    {
        public IFormFile PostImage { get; set; }
    }
}
