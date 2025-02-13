using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses
{
    public class PostWithaSelectedCommentResponseDto
    {
        public PostResponseDto Post { get; set; }
        public PostCommentResponseDto SelectedComment { get; set; }
    }
}
