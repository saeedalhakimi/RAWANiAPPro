using RAWANi.WEBAPi.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses
{
    public record PostWithCommentsResponseDto
    {
        public PostResponseDto Post { get; set; }
        public PagedResponse<PostCommentResponseDto> Comments { get; set; }
    }
}
