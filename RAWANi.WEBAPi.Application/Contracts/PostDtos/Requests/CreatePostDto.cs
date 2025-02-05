﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.PostDtos.Requests
{
    public record CreatePostDto
    {
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public IFormFile PostImage { get; set; }
    }
}
