using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Domain.Entities.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.PostDtos
{
    public static class PostMappers
    {
        public static PostResponseDto ToPostResponseDto(Post post)
        {
            return new PostResponseDto
            {
                PostID = post.PostID.Value,
                UserProfileID = post.UserProfileID.Value,
                PostTilte  = post.PostTitle.Value,
                PostContent = post.PostContent.Value,
                PostImageLink = post.ImageLink,
                CreateAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
            };
        }
    }
}
