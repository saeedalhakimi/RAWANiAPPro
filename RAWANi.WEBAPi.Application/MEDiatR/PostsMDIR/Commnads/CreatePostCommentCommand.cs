using MediatR;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads
{
    public class CreatePostCommentCommand : IRequest<OperationResult<PostCommentResponseDto>>
    {
        public Guid PostID { get; set; }
        public Guid UserProfileID { get; set; }
        public string CommentContent { get; set; }
    }
}
