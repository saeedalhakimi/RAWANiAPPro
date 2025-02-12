using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.PostDtos.Requests
{
    public record CreatePostCommentDto
    {
        [Required(ErrorMessage = "Comment cannot be empty.")]
        [MaxLength(255)]
        public string CommentContent { get; set; }
    }
}
