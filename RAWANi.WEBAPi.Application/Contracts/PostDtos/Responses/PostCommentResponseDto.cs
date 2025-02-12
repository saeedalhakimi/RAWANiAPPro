using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses
{
    public record PostCommentResponseDto
    {
        public Guid CommentID { get; set; }
        public Guid PostID { get; set; }
        public Guid UserProfileID { get; set; }
        public string CommentContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
