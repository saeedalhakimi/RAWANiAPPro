using MediatR;
using Microsoft.AspNetCore.Http;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Requests;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Domain.Entities.Posts;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads
{
    public class CreatePostCommand : IRequest<OperationResult<PostResponseDto>>
    {
        public Guid UserProfileID { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public IFormFile? PostImage { get; set; }
    }
}
